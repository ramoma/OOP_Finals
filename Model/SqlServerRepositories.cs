#nullable disable
using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;
using Microsoft.Data.SqlClient;   // dotnet add package Microsoft.Data.SqlClient

namespace Model;

internal static class Sql
{
    public static async Task<SqlConnection> OpenAsync(string connectionString)
    {
        var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();
        return conn;
    }

    public static SqlParameter P(string name, SqlDbType type, object value)
    {
        return new SqlParameter(name, type) { Value = value ?? DBNull.Value };
    }

    public static SqlCommand Cmd(SqlConnection conn, SqlTransaction tx, string sql, params SqlParameter[] ps)
    {
        var cmd = new SqlCommand(sql, conn, tx);
        if (ps != null && ps.Length > 0) cmd.Parameters.AddRange(ps);
        return cmd;
    }

    public static async Task<object> ScalarAsync(SqlConnection conn, SqlTransaction tx, string sql, params SqlParameter[] ps)
    {
        using (var cmd = Cmd(conn, tx, sql, ps))
        return await cmd.ExecuteScalarAsync();
    }

    public static async Task<int> ExecAsync(SqlConnection conn, SqlTransaction tx, string sql, params SqlParameter[] ps)
    {
        using (var cmd = Cmd(conn, tx, sql, ps))
        return await cmd.ExecuteNonQueryAsync();
    }

    public static string Str(SqlDataReader r, int i)
    {
        return r.IsDBNull(i) ? null : r.GetString(i);
    }
}

// ======================================================================
//  purchase_orders / purchase_order_items / inventory_batches / stock_movements
// ======================================================================
public sealed class SqlServerPurchaseOrderRepository : IPurchaseOrderRepository
{
    private readonly string _cs;

    public SqlServerPurchaseOrderRepository(string connectionString)
    {
        _cs = connectionString;
    }

    private static PurchaseOrderStatus ParseStatus(string s)
    {
        switch (s)
        {
            case "DRAFT": return PurchaseOrderStatus.Draft;
            case "ORDERED": return PurchaseOrderStatus.Ordered;
            case "PARTIALLY_RECEIVED": return PurchaseOrderStatus.PartiallyReceived;
            case "RECEIVED": return PurchaseOrderStatus.Received;
            case "CANCELLED": return PurchaseOrderStatus.Cancelled;
            default: throw new InvalidOperationException("Unknown purchase order status: " + s);
        }
    }

    // ------------------------------------------------------------------
    //  List for the table
    // ------------------------------------------------------------------
    public async Task<IReadOnlyList<PurchaseOrderSummary>> GetInboundAsync()
    {
        const string sql = @"
        SELECT po.id, po.po_number, s.company_name, po.status,
        po.expected_delivery_date, po.created_at, po.total_cost,
        totals.line_count, totals.total_qty,
        first_line.product_name, first_line.assigned_lot_number
        FROM purchase_orders po
        JOIN suppliers s ON s.id = po.supplier_id
        OUTER APPLY (
            SELECT COUNT(*) AS line_count,
                     COALESCE(SUM(i.quantity_ordered), 0) AS total_qty
                     FROM purchase_order_items i
                     WHERE i.purchase_order_id = po.id
        ) totals
        OUTER APPLY (
            SELECT TOP (1) p.name AS product_name, i.assigned_lot_number
            FROM purchase_order_items i
            JOIN products p ON p.id = i.product_id
            WHERE i.purchase_order_id = po.id
            ORDER BY p.name
        ) first_line
        WHERE po.status <> 'CANCELLED'
        ORDER BY po.created_at DESC";

        var list = new List<PurchaseOrderSummary>();

        using (var conn = await Sql.OpenAsync(_cs))
        using (var cmd = Sql.Cmd(conn, null, sql))
        using (var r = await cmd.ExecuteReaderAsync())
        {
            while (await r.ReadAsync())
            {
                list.Add(new PurchaseOrderSummary
                {
                    Id = r.GetGuid(0),
                         PoNumber = r.GetString(1),
                         SupplierName = r.GetString(2),
                         Status = ParseStatus(r.GetString(3)),
                         ExpectedOn = r.IsDBNull(4) ? (DateTime?)null : r.GetDateTime(4),
                         OrderedOn = r.GetDateTime(5),
                         TotalCost = r.GetDecimal(6),
                         LineCount = r.GetInt32(7),
                         TotalQty = r.GetInt32(8),
                         FirstItemName = Sql.Str(r, 9) ?? "(no items)",
                         FirstLot = Sql.Str(r, 10)
                });
            }
        }

        return list;
    }

    // ------------------------------------------------------------------
    //  Lines for the receive dialog
    // ------------------------------------------------------------------
    public async Task<IReadOnlyList<ReceivingLine>> GetReceivingLinesAsync(Guid purchaseOrderId)
    {
        const string sql = @"
        SELECT i.id, i.product_id, p.name, p.requires_expiry,
        i.quantity_ordered, i.quantity_received
        FROM purchase_order_items i
        JOIN products p ON p.id = i.product_id
        WHERE i.purchase_order_id = @po
        ORDER BY p.name";

            var list = new List<ReceivingLine>();

            using (var conn = await Sql.OpenAsync(_cs))
            using (var cmd = Sql.Cmd(conn, null, sql, Sql.P("@po", SqlDbType.UniqueIdentifier, purchaseOrderId)))
            using (var r = await cmd.ExecuteReaderAsync())
            {
                while (await r.ReadAsync())
                {
                    list.Add(new ReceivingLine
                    {
                        PurchaseOrderItemId = r.GetGuid(0),
                             ProductId = r.GetGuid(1),
                             ProductName = r.GetString(2),
                             RequiresExpiry = r.GetBoolean(3),
                             QuantityOrdered = r.GetInt32(4),
                             QuantityReceived = r.GetInt32(5)
                    });
                }
            }

            return list;
    }

    // ------------------------------------------------------------------
    //  Receiving: everything below succeeds together, or nothing is saved
    // ------------------------------------------------------------------
    public async Task ReceiveAsync(Guid purchaseOrderId, Guid userId, IReadOnlyList<ReceiptInput> receipts)
    {
        if (receipts == null || receipts.Count == 0)
            throw new ArgumentException("There is nothing to receive.");

        using (var conn = await Sql.OpenAsync(_cs))
        using (var tx = conn.BeginTransaction())
        {
            var poParam = Sql.P("@po", SqlDbType.UniqueIdentifier, purchaseOrderId);

            // 1) Lock and validate the purchase order.
            string poNumber, status;
            using (var cmd = Sql.Cmd(conn, tx,
                                     "SELECT po_number, status FROM purchase_orders WITH (UPDLOCK, ROWLOCK) WHERE id = @po",
                                     Sql.P("@po", SqlDbType.UniqueIdentifier, purchaseOrderId)))
            using (var r = await cmd.ExecuteReaderAsync())
            {
                if (!await r.ReadAsync())
                    throw new InvalidOperationException("Purchase order not found.");
                poNumber = r.GetString(0);
                status = r.GetString(1);
            }

            if (status == "DRAFT") throw new InvalidOperationException(poNumber + " is still a draft. Mark it as ordered before receiving.");
            if (status == "RECEIVED") throw new InvalidOperationException(poNumber + " was already fully received.");
            if (status == "CANCELLED") throw new InvalidOperationException(poNumber + " was cancelled.");

            int processed = 0;

            foreach (var rc in receipts)
            {
                if (rc.QuantityReceived <= 0) continue;
                if (string.IsNullOrWhiteSpace(rc.LotNumber))
                    throw new InvalidOperationException("A lot number is required for every item you receive.");

                string lot = rc.LotNumber.Trim();

                // 2) Lock and validate the line.
                Guid productId;
                int ordered, received;
                bool requiresExpiry;
                using (var cmd = Sql.Cmd(conn, tx, @"
                SELECT i.product_id, i.quantity_ordered, i.quantity_received, p.requires_expiry
                FROM purchase_order_items AS i WITH (UPDLOCK, ROWLOCK)
                JOIN products p ON p.id = i.product_id
                WHERE i.id = @item AND i.purchase_order_id = @po",
                Sql.P("@item", SqlDbType.UniqueIdentifier, rc.PurchaseOrderItemId),
                                         Sql.P("@po", SqlDbType.UniqueIdentifier, purchaseOrderId)))
                using (var r = await cmd.ExecuteReaderAsync())
                {
                    if (!await r.ReadAsync())
                        throw new InvalidOperationException("A line item does not belong to this purchase order.");
                    productId = r.GetGuid(0);
                    ordered = r.GetInt32(1);
                    received = r.GetInt32(2);
                    requiresExpiry = r.GetBoolean(3);
                }

                if (rc.QuantityReceived > ordered - received)
                    throw new InvalidOperationException("You cannot receive more than the quantity still outstanding.");
                if (requiresExpiry && !rc.ExpiryDate.HasValue)
                    throw new InvalidOperationException("An expiry date is required for this product.");

                var pParam = Sql.P("@p", SqlDbType.UniqueIdentifier, productId);
                var qtyParam = Sql.P("@qty", SqlDbType.Int, rc.QuantityReceived);

                // 3) Add to the batch with the same lot number, or create a new batch.
                Guid batchId;
                object existing = await Sql.ScalarAsync(conn, tx,
                                                        "SELECT id FROM inventory_batches WITH (UPDLOCK, ROWLOCK) WHERE product_id = @p AND lot_number = @lot",
                                                        Sql.P("@p", SqlDbType.UniqueIdentifier, productId),
                                                        Sql.P("@lot", SqlDbType.NVarChar, lot));

                if (existing != null && existing != DBNull.Value)
                {
                    batchId = (Guid)existing;
                    await Sql.ExecAsync(conn, tx,
                                        "UPDATE inventory_batches SET quantity_on_hand = quantity_on_hand + @qty WHERE id = @b",
                                        Sql.P("@qty", SqlDbType.Int, rc.QuantityReceived),
                                        Sql.P("@b", SqlDbType.UniqueIdentifier, batchId));
                }
                else
                {
                    object newId = await Sql.ScalarAsync(conn, tx, @"
                    INSERT INTO inventory_batches (product_id, lot_number, quantity_on_hand, expiry_date, status)
                    OUTPUT INSERTED.id
                    VALUES (@p, @lot, @qty, @exp, 'ACTIVE')",
                                                         Sql.P("@p", SqlDbType.UniqueIdentifier, productId),
                                                         Sql.P("@lot", SqlDbType.NVarChar, lot),
                                                         Sql.P("@qty", SqlDbType.Int, rc.QuantityReceived),
                                                         Sql.P("@exp", SqlDbType.Date, rc.ExpiryDate.HasValue ? (object)rc.ExpiryDate.Value.Date : null));
                    batchId = (Guid)newId;
                }

                // 4) Update the PO line.
                await Sql.ExecAsync(conn, tx, @"
                UPDATE purchase_order_items
                SET quantity_received = quantity_received + @qty,
                assigned_lot_number = @lot,
                assigned_expiry_date = @exp
                WHERE id = @item",
                Sql.P("@qty", SqlDbType.Int, rc.QuantityReceived),
                                    Sql.P("@lot", SqlDbType.NVarChar, lot),
                                    Sql.P("@exp", SqlDbType.Date, rc.ExpiryDate.HasValue ? (object)rc.ExpiryDate.Value.Date : null),
                                    Sql.P("@item", SqlDbType.UniqueIdentifier, rc.PurchaseOrderItemId));

                // 5) Total stock for the product right now (sum of ACTIVE batches), for the audit snapshot.
                int balanceAfter = Convert.ToInt32(await Sql.ScalarAsync(conn, tx,
                                                                         "SELECT COALESCE(SUM(quantity_on_hand), 0) FROM inventory_batches WHERE product_id = @p AND status = 'ACTIVE'",
                                                                         Sql.P("@p", SqlDbType.UniqueIdentifier, productId)));

                // 6) Append to the immutable audit log.
                await Sql.ExecAsync(conn, tx, @"
                INSERT INTO stock_movements
                (product_id, batch_id, user_id, movement_type, quantity_change, balance_after, reference_id, reason_notes)
                VALUES
                (@p, @b, @u, 'PO_RECEIPT', @qty, @bal, @ref, @notes)",
                                    Sql.P("@p", SqlDbType.UniqueIdentifier, productId),
                                    Sql.P("@b", SqlDbType.UniqueIdentifier, batchId),
                                    Sql.P("@u", SqlDbType.UniqueIdentifier, userId),
                                    Sql.P("@qty", SqlDbType.Int, rc.QuantityReceived),
                                    Sql.P("@bal", SqlDbType.Int, balanceAfter),
                                    Sql.P("@ref", SqlDbType.NVarChar, poNumber),
                                    Sql.P("@notes", SqlDbType.NVarChar, "Received against " + poNumber));

                processed++;
            }

            if (processed == 0)
                throw new InvalidOperationException("Enter a quantity for at least one item.");

            // 7) Fully received, or partially?
            int outstanding = Convert.ToInt32(await Sql.ScalarAsync(conn, tx,
                                                                    "SELECT COUNT(*) FROM purchase_order_items WHERE purchase_order_id = @po AND quantity_received < quantity_ordered",
                                                                    poParam));

            if (outstanding == 0)
            {
                await Sql.ExecAsync(conn, tx,
                                    "UPDATE purchase_orders SET status = 'RECEIVED', received_date = SYSUTCDATETIME() WHERE id = @po",
                                    Sql.P("@po", SqlDbType.UniqueIdentifier, purchaseOrderId));
            }
            else
            {
                await Sql.ExecAsync(conn, tx,
                                    "UPDATE purchase_orders SET status = 'PARTIALLY_RECEIVED' WHERE id = @po",
                                    Sql.P("@po", SqlDbType.UniqueIdentifier, purchaseOrderId));
            }

            tx.Commit();   // leaving the using block without Commit() rolls everything back
        }
    }
}

// ======================================================================
//  suppliers
// ======================================================================
public sealed class SqlServerSupplierRepository : ISupplierRepository
{
    private readonly string _cs;

    public SqlServerSupplierRepository(string connectionString)
    {
        _cs = connectionString;
    }

    public async Task<IReadOnlyList<SupplierEntity>> GetAllAsync()
    {
        const string sql = @"
        SELECT id, company_name, contact_name, email, phone, lead_time_days, payment_terms
        FROM suppliers
        ORDER BY company_name";

        var list = new List<SupplierEntity>();

        using (var conn = await Sql.OpenAsync(_cs))
        using (var cmd = Sql.Cmd(conn, null, sql))
        using (var r = await cmd.ExecuteReaderAsync())
        {
            while (await r.ReadAsync())
            {
                list.Add(new SupplierEntity
                {
                    Id = r.GetGuid(0),
                         CompanyName = r.GetString(1),
                         ContactName = Sql.Str(r, 2),
                         Email = Sql.Str(r, 3),
                         Phone = Sql.Str(r, 4),
                         LeadTimeDays = r.IsDBNull(5) ? (int?)null : r.GetInt32(5),
                         PaymentTerms = Sql.Str(r, 6)
                });
            }
        }

        return list;
    }
}
