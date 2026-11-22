#nullable disable
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using Model;
using View;

namespace Controls;

/// <summary>
/// Connects SuppliersPanel (view) to the repositories (model).
/// The view raises events, the controller talks to the database, then pushes results back to the view.
/// </summary>
public sealed class SuppliersController
{
    private readonly Suppliers _view;
    private readonly IPurchaseOrderRepository _orders;
    private readonly ISupplierRepository _suppliers;
    private readonly Guid _currentUserId;     // users.id of whoever is logged in (needed for the audit log)

    public SuppliersController(Suppliers view,
                               IPurchaseOrderRepository orders,
                               ISupplierRepository suppliers,
                               Guid currentUserId)
    {
        _view = view;
        _orders = orders;
        _suppliers = suppliers;
        _currentUserId = currentUserId;

        _view.MarkReceivedRequested += OnMarkReceivedRequested;
        _view.CreatePurchaseOrderRequested += OnCreatePurchaseOrderRequested;
        _view.NewPurchaseOrderForVendorRequested += OnNewPurchaseOrderForVendorRequested;
    }

    // ------------------------------------------------------------------
    //  Load / refresh everything shown on the panel
    // ------------------------------------------------------------------
    public async Task LoadAsync()
    {
        try
        {
            var orders = await _orders.GetInboundAsync();
            var suppliers = await _suppliers.GetAllAsync();

            _view.SetPurchaseOrders(orders.Select(ToRow));
            _view.SetVendors(suppliers.Select((s, i) => ToRow(s, i)));
        }
        catch (Exception ex)
        {
            ShowError("Could not load suppliers", ex);
        }
    }

    // ------------------------------------------------------------------
    //  View events -> database
    // ------------------------------------------------------------------
    private async void OnMarkReceivedRequested(object sender, PoEventArgs e)
    {
        try
        {
            if (e.Order.Status == PoStatus.Pending)
            {
                // Pending = DRAFT in the database.
                MessageBox.Show(_view.FindForm(),
                                e.Order.Number + " is still a draft. Mark it as ordered before receiving stock.",
                                "Draft purchase order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            Guid poId = (Guid)e.Order.Tag;                       // set in ToRow(...)
            var lines = await _orders.GetReceivingLinesAsync(poId);

            if (lines.Count == 0)
            {
                MessageBox.Show(_view.FindForm(), "This purchase order has no line items.",
                                "Nothing to receive", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            using (var dialog = new ReceivePurchaseOrderForm(e.Order.Number, lines))
            {
                if (dialog.ShowDialog(_view.FindForm()) != DialogResult.OK) return;

                // batches + PO lines + audit log + PO status, in one transaction
                await _orders.ReceiveAsync(poId, _currentUserId, dialog.Result);
            }

            // The PO may now be RECEIVED or PARTIALLY_RECEIVED, so reload and show what the database says.
            await LoadAsync();
        }
        catch (Exception ex)
        {
            ShowError("Could not receive the purchase order", ex);
        }
    }

    private void OnCreatePurchaseOrderRequested(object sender, EventArgs e)
    {
        OpenCreateDialog(null);
    }

    private void OnNewPurchaseOrderForVendorRequested(object sender, VendorEventArgs e)
    {
        OpenCreateDialog((Guid)e.Vendor.Tag);                    // suppliers.id
    }

    // Replace the body with your real "create purchase order" form.
    // A new PO needs: a purchase_orders row (po_number, supplier_id, status, expected_delivery_date,
    // total_cost, created_by = _currentUserId) plus its purchase_order_items rows.
    private void OpenCreateDialog(Guid? supplierId)
    {
        MessageBox.Show(_view.FindForm(),
                        supplierId.HasValue ? "Create a PO for supplier " + supplierId.Value : "Create a new PO",
                        "Create Purchase Order");
    }

    // ------------------------------------------------------------------
    //  Entity -> view model mapping
    // ------------------------------------------------------------------
    private static PoRow ToRow(PurchaseOrderSummary p)
    {
        return new PoRow
        {
            Number = p.PoNumber,
            Supplier = p.SupplierName,
            Item = p.LineCount > 1 ? p.FirstItemName + " +" + (p.LineCount - 1) + " more" : p.FirstItemName,
            Lot = string.IsNullOrEmpty(p.FirstLot) ? "\u2014" : p.FirstLot,
            Qty = p.TotalQty,
            Cost = p.TotalCost,
            Ordered = p.OrderedOn,
            Expected = p.ExpectedOn,
            Status = ToViewStatus(p.Status),
            Tag = p.Id                                           // the panel hands this back in events
        };
    }

    // The suppliers table has no code, rating or city, so:
    //   Code   -> generated from the list position (SUP-01, SUP-02, ...)  [display only, not stable]
    //   Rating -> 0, and the panel hides the star when it is 0
    //   City   -> shows the payment terms instead (the small text at the bottom-left of the card)
    private static VendorRow ToRow(SupplierEntity s, int index)
    {
        return new VendorRow
        {
            Code = "SUP-" + (index + 1).ToString("00"),
            Name = s.CompanyName,
            Contact = s.ContactName,
            LeadTime = s.LeadTimeDays.HasValue ? s.LeadTimeDays.Value + " Days" : "",
            Email = s.Email,
            Phone = s.Phone,
            Rating = 0,
            City = s.PaymentTerms,
            Tag = s.Id
        };
    }

    // DRAFT                      -> Pending     (grey pill)
    // ORDERED, PARTIALLY_RECEIVED -> In Transit (teal pill, can still be received)
    // RECEIVED                   -> Received    (green pill)
    private static PoStatus ToViewStatus(PurchaseOrderStatus s)
    {
        switch (s)
        {
            case PurchaseOrderStatus.Ordered:
            case PurchaseOrderStatus.PartiallyReceived:
                return PoStatus.InTransit;
            case PurchaseOrderStatus.Received:
                return PoStatus.Received;
            default:
                return PoStatus.Pending;
        }
    }

    private void ShowError(string title, Exception ex)
    {
        MessageBox.Show(_view.FindForm(), ex.Message, title, MessageBoxButtons.OK, MessageBoxIcon.Error);
    }
}
