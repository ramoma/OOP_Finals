#nullable disable
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;
using Model;

namespace View;

/// <summary>
/// Shown when the user clicks "Mark Received". For each PO line it asks how many units arrived,
/// the manufacturer's lot number, and (for consumables) the expiry date.
/// </summary>
public sealed class ReceivePurchaseOrderForm : Form
{
    private const int ColQtyNow = 3;
    private const int ColLot = 4;
    private const int ColExpiry = 5;

    private readonly IReadOnlyList<ReceivingLine> _lines;
    private readonly DataGridView _grid;
    private readonly Label _error;

    /// <summary>The validated entries. Only valid after the dialog returns DialogResult.OK.</summary>
    [Browsable(false)]
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public IReadOnlyList<ReceiptInput> Result { get; private set; }

    public ReceivePurchaseOrderForm(string poNumber, IReadOnlyList<ReceivingLine> lines)
    {
        _lines = lines;

        Text = "Receive " + poNumber;
        StartPosition = FormStartPosition.CenterParent;
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        ClientSize = new Size(820, 380);

        // ---- instructions (top) ----
        var info = new Label
        {
            Text = "Enter how many units arrived and the lot number for each item. " +
            "An expiry date (yyyy-MM-dd) is required for items that expire.",
            Dock = DockStyle.Top,
            Height = 44,
            Padding = new Padding(12, 12, 12, 0),
            UseMnemonic = false
        };

        // ---- grid (fill) ----
        _grid = new DataGridView
        {
            Dock = DockStyle.Fill,
            AllowUserToAddRows = false,
            AllowUserToDeleteRows = false,
            AllowUserToResizeRows = false,
            RowHeadersVisible = false,
            BackgroundColor = SystemColors.Window,
            BorderStyle = BorderStyle.None,
            SelectionMode = DataGridViewSelectionMode.CellSelect,
            EditMode = DataGridViewEditMode.EditOnEnter,
            AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
            MultiSelect = false
        };

        AddColumn("Product", 34, true);
        AddColumn("Ordered", 10, true);
        AddColumn("Already received", 14, true);
        AddColumn("Receive now", 12, false);
        AddColumn("Lot number", 18, false);
        AddColumn("Expiry (yyyy-MM-dd)", 18, false);

        for (int i = 0; i < lines.Count; i++)
        {
            var l = lines[i];
            int idx = _grid.Rows.Add(
                l.ProductName,
                l.QuantityOrdered,
                l.QuantityReceived,
                l.Remaining > 0 ? l.Remaining.ToString(CultureInfo.InvariantCulture) : "0",
                                     "",
                                     l.RequiresExpiry ? "" : "n/a");

            var row = _grid.Rows[idx];

            if (l.Remaining <= 0)
            {
                // Nothing left to receive on this line.
                row.Cells[ColQtyNow].ReadOnly = true;
                row.Cells[ColLot].ReadOnly = true;
                row.Cells[ColExpiry].ReadOnly = true;
                row.DefaultCellStyle.ForeColor = SystemColors.GrayText;
            }
            else if (!l.RequiresExpiry)
            {
                row.Cells[ColExpiry].ReadOnly = true;
                row.Cells[ColExpiry].Style.ForeColor = SystemColors.GrayText;
            }
        }

        // ---- buttons (bottom) ----
        var bottom = new Panel { Dock = DockStyle.Bottom, Height = 56 };

        _error = new Label
        {
            AutoSize = false,
            ForeColor = Color.Firebrick,
            Location = new Point(12, 10),
            Size = new Size(560, 36),
            TextAlign = ContentAlignment.MiddleLeft,
            UseMnemonic = false
        };

        var cancel = new Button
        {
            Text = "Cancel",
            DialogResult = DialogResult.Cancel,
            Size = new Size(90, 32),
            Location = new Point(ClientSize.Width - 12 - 90, 12)
        };

        var ok = new Button
        {
            Text = "Receive",
            Size = new Size(100, 32),
            Location = new Point(cancel.Left - 8 - 100, 12)
        };
        ok.Click += OnOk;

        bottom.Controls.Add(_error);
        bottom.Controls.Add(ok);
        bottom.Controls.Add(cancel);

        CancelButton = cancel;

        // Fill first, then the edge-docked controls, so docking resolves correctly.
        Controls.Add(_grid);
        Controls.Add(info);
        Controls.Add(bottom);
    }

    private void AddColumn(string header, float weight, bool readOnly)
    {
        _grid.Columns.Add(new DataGridViewTextBoxColumn
        {
            HeaderText = header,
            FillWeight = weight,
            ReadOnly = readOnly,
            SortMode = DataGridViewColumnSortMode.NotSortable
        });
    }

    private void Fail(int rowIndex, int colIndex, string message)
    {
        _error.Text = message;
        _grid.CurrentCell = _grid.Rows[rowIndex].Cells[colIndex];
        _grid.BeginEdit(true);
    }

    private void OnOk(object sender, EventArgs e)
    {
        _error.Text = "";
        _grid.EndEdit();

        var results = new List<ReceiptInput>();

        for (int i = 0; i < _lines.Count; i++)
        {
            var line = _lines[i];
            if (line.Remaining <= 0) continue;

            var row = _grid.Rows[i];

            int qty;
            string qtyText = Convert.ToString(row.Cells[ColQtyNow].Value, CultureInfo.InvariantCulture);
            if (!int.TryParse(qtyText, NumberStyles.Integer, CultureInfo.InvariantCulture, out qty) || qty < 0)
            {
                Fail(i, ColQtyNow, "\"Receive now\" must be a whole number (0 or more).");
                return;
            }

            if (qty == 0) continue;   // nothing arrived for this line

            if (qty > line.Remaining)
            {
                Fail(i, ColQtyNow, line.ProductName + ": only " + line.Remaining + " unit(s) are still outstanding.");
                return;
            }

            string lot = (Convert.ToString(row.Cells[ColLot].Value) ?? "").Trim();
            if (lot.Length == 0)
            {
                Fail(i, ColLot, line.ProductName + ": enter the lot number.");
                return;
            }
            if (lot.Length > 50)
            {
                Fail(i, ColLot, "Lot numbers can be at most 50 characters.");
                return;
            }

            DateTime? expiry = null;
            if (line.RequiresExpiry)
            {
                DateTime parsed;
                string text = (Convert.ToString(row.Cells[ColExpiry].Value) ?? "").Trim();
                if (!DateTime.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out parsed))
                {
                    Fail(i, ColExpiry, line.ProductName + ": enter the expiry date as yyyy-MM-dd.");
                    return;
                }
                expiry = parsed.Date;
            }

            results.Add(new ReceiptInput
            {
                PurchaseOrderItemId = line.PurchaseOrderItemId,
                QuantityReceived = qty,
                LotNumber = lot,
                ExpiryDate = expiry
            });
        }

        if (results.Count == 0)
        {
            _error.Text = "Enter a quantity for at least one item.";
            return;
        }

        Result = results;
        DialogResult = DialogResult.OK;
    }
}
