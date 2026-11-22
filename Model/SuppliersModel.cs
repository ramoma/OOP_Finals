#nullable disable
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Model;

// ----------------------------------------------------------------------
//  Entities (shaped by your tables; independent of the UI panel)
// ----------------------------------------------------------------------

/// <summary>purchase_orders.status: DRAFT, ORDERED, PARTIALLY_RECEIVED, RECEIVED, CANCELLED</summary>
public enum PurchaseOrderStatus { Draft, Ordered, PartiallyReceived, Received, Cancelled }

/// <summary>One row of the "Inbound Purchase Orders" table (one PO, its lines summarized).</summary>
public sealed class PurchaseOrderSummary
{
    public Guid Id { get; set; }                  // purchase_orders.id
    public string PoNumber { get; set; }
    public string SupplierName { get; set; }      // suppliers.company_name
    public string FirstItemName { get; set; }     // first line's products.name
    public string FirstLot { get; set; }          // first line's assigned_lot_number (null until received)
    public int LineCount { get; set; }
    public int TotalQty { get; set; }             // SUM(quantity_ordered)
    public decimal TotalCost { get; set; }        // purchase_orders.total_cost
    public DateTime OrderedOn { get; set; }       // purchase_orders.created_at
    public DateTime? ExpectedOn { get; set; }     // purchase_orders.expected_delivery_date
    public PurchaseOrderStatus Status { get; set; }
}

/// <summary>One purchase_order_items line, shown in the "receive" dialog.</summary>
public sealed class ReceivingLine
{
    public Guid PurchaseOrderItemId { get; set; }
    public Guid ProductId { get; set; }
    public string ProductName { get; set; }
    public bool RequiresExpiry { get; set; }      // products.requires_expiry
    public int QuantityOrdered { get; set; }
    public int QuantityReceived { get; set; }
    public int Remaining { get { return QuantityOrdered - QuantityReceived; } }
}

/// <summary>What the user typed for one line when clicking "Mark Received".</summary>
public sealed class ReceiptInput
{
    public Guid PurchaseOrderItemId { get; set; }
    public int QuantityReceived { get; set; }     // units that arrived now
    public string LotNumber { get; set; }         // -> assigned_lot_number + inventory_batches.lot_number
    public DateTime? ExpiryDate { get; set; }     // required when products.requires_expiry = 1
}

public sealed class SupplierEntity
{
    public Guid Id { get; set; }
    public string CompanyName { get; set; }
    public string ContactName { get; set; }
    public string Email { get; set; }
    public string Phone { get; set; }
    public int? LeadTimeDays { get; set; }
    public string PaymentTerms { get; set; }
}

// ----------------------------------------------------------------------
//  Repository interfaces: the controller depends on these only.
// ----------------------------------------------------------------------
public interface IPurchaseOrderRepository
{
    /// <summary>All non-cancelled purchase orders, newest first.</summary>
    Task<IReadOnlyList<PurchaseOrderSummary>> GetInboundAsync();

    /// <summary>The line items of one PO (for the receive dialog).</summary>
    Task<IReadOnlyList<ReceivingLine>> GetReceivingLinesAsync(Guid purchaseOrderId);

    /// <summary>
    /// Receives stock in ONE transaction: creates/updates inventory_batches, updates
    /// purchase_order_items, writes stock_movements (PO_RECEIPT) and updates the PO status.
    /// </summary>
    Task ReceiveAsync(Guid purchaseOrderId, Guid userId, IReadOnlyList<ReceiptInput> receipts);
}

public interface ISupplierRepository
{
    Task<IReadOnlyList<SupplierEntity>> GetAllAsync();
}
