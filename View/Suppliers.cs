using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;

namespace View  // <- change to your project's namespace
{
    // ======================================================================
    //  Public models: map your database entities onto these
    // ======================================================================
    public enum PoStatus { InTransit, Pending, Received }

    public sealed class PoRow
    {
        public string Number { get; set; }       // e.g. "PO-2026-010"
        public string Supplier { get; set; }
        public string Item { get; set; }
        public string Lot { get; set; }
        public int Qty { get; set; }
        public decimal Cost { get; set; }
        public DateTime Ordered { get; set; }
        public DateTime Expected { get; set; }
        public PoStatus Status { get; set; }
        public object Tag { get; set; }          // optional: your DB id / entity
    }

    public sealed class VendorRow
    {
        public string Code { get; set; }         // e.g. "SUP-01"
        public string Name { get; set; }
        public string Contact { get; set; }
        public string LeadTime { get; set; }     // e.g. "2-4 Days"
        public string Email { get; set; }
        public string Phone { get; set; }
        public double Rating { get; set; }
        public string City { get; set; }
        public object Tag { get; set; }          // optional: your DB id / entity
    }

    public sealed class PoEventArgs : EventArgs
    {
        public PoEventArgs(PoRow order) { Order = order; }
        public PoRow Order { get; private set; }
    }

    public sealed class VendorEventArgs : EventArgs
    {
        public VendorEventArgs(VendorRow vendor) { Vendor = vendor; }
        public VendorRow Vendor { get; private set; }
    }

    /// <summary>
    /// "Suppliers & Purchase Orders" screen as a drop-in Panel. It holds no data of its own.
    ///
    ///   var p = new SuppliersPanel { Dock = DockStyle.Fill };
    ///   p.MarkReceivedRequested += (s, e) => { /* controller updates DB, then: */
    ///                                          p.SetPurchaseOrderStatus(e.Order.Number, PoStatus.Received); };
    ///   p.SetPurchaseOrders(rowsFromDb);
    ///   p.SetVendors(vendorsFromDb);
    /// </summary>
    public sealed class Suppliers : Panel
    {
        // ==================================================================
        //  Public API
        // ==================================================================

        /// <summary>"Create Purchase Order" header button clicked.</summary>
        public event EventHandler CreatePurchaseOrderRequested;

        /// <summary>"New PO" link on a vendor card clicked.</summary>
        public event EventHandler<VendorEventArgs> NewPurchaseOrderForVendorRequested;

        /// <summary>
        /// "Mark Received" clicked. The panel does NOT change the row itself: update your database,
        /// then call SetPurchaseOrderStatus (or SetPurchaseOrders to reload).
        /// </summary>
        public event EventHandler<PoEventArgs> MarkReceivedRequested;

        /// <summary>Replaces the whole PO table. Safe to call from any thread.</summary>
        public void SetPurchaseOrders(IEnumerable<PoRow> orders)
        {
            var list = orders == null ? new List<PoRow>() : new List<PoRow>(orders);
            if (InvokeRequired) { BeginInvoke(new Action(() => SetPurchaseOrders(list))); return; }

            _orders.Clear();
            _orders.AddRange(list);
            _hoverRow = -1;
            _hotRow = -1;

            _grid.RowCount = 0;
            _grid.RowCount = _orders.Count;
            _ordersEmpty.Visible = _orders.Count == 0;

            LayoutUi();
            ApplyGridRegion();
            _grid.Invalidate();
        }

        /// <summary>Replaces the vendor cards. Safe to call from any thread.</summary>
        public void SetVendors(IEnumerable<VendorRow> vendors)
        {
            var list = vendors == null ? new List<VendorRow>() : new List<VendorRow>(vendors);
            if (InvokeRequired) { BeginInvoke(new Action(() => SetVendors(list))); return; }

            _vendors.Clear();
            _vendors.AddRange(list);
            _vendorsEmpty.Visible = _vendors.Count == 0;

            RebuildVendorCards();
            LayoutUi();
        }

        /// <summary>Changes one PO's status (after your DB update succeeded). Safe from any thread.</summary>
        public void SetPurchaseOrderStatus(string poNumber, PoStatus status)
        {
            if (InvokeRequired) { BeginInvoke(new Action(() => SetPurchaseOrderStatus(poNumber, status))); return; }

            for (int i = 0; i < _orders.Count; i++)
            {
                if (string.Equals(_orders[i].Number, poNumber, StringComparison.OrdinalIgnoreCase))
                {
                    _orders[i].Status = status;
                    _grid.InvalidateRow(i);
                }
            }
        }

        // ==================================================================
        //  Theme  (PageBg should match your main form's content background)
        // ==================================================================
        private static class Theme
        {
            // --- fonts that exist on this machine (falls back gracefully) ---
            private static readonly HashSet<string> Installed = LoadInstalled();
            private static readonly string Sans = Pick("Segoe UI", "Inter", "Noto Sans", "DejaVu Sans", "Liberation Sans", "Arial");
            private static readonly string MonoName = Pick("Consolas", "Cascadia Mono", "DejaVu Sans Mono", "Liberation Mono", "Courier New");
            private static readonly bool HasSemibold = Installed.Contains("Segoe UI Semibold");

            private static HashSet<string> LoadInstalled()
            {
                var set = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
                try
                {
                    using (var fc = new InstalledFontCollection())
                    foreach (var f in fc.Families) set.Add(f.Name);
                }
                catch { }
                return set;
            }

            private static string Pick(params string[] names)
            {
                foreach (var n in names) if (Installed.Contains(n)) return n;
                return SystemFonts.DefaultFont.FontFamily.Name;
            }

            private static Font Strong(float size)
            {
                return HasSemibold ? new Font("Segoe UI Semibold", size) : new Font(Sans, size, FontStyle.Bold);
            }

            private static Color C(string hex) { return ColorTranslator.FromHtml(hex); }

            public static readonly Color PageBg = C("#F5F5F4");
            public static readonly Color Ink = C("#111827");
            public static readonly Color BodyText = C("#334155");
            public static readonly Color Muted = C("#64748B");
            public static readonly Color Faint = C("#94A3B8");
            public static readonly Color Border = C("#E5E7EB");
            public static readonly Color RowLine = C("#F0F0EE");
            public static readonly Color HeaderBg = C("#FAFAF9");
            public static readonly Color RowHover = C("#FBFBFA");

            public static readonly Color Teal = C("#0F9AA0");
            public static readonly Color TealHover = C("#0C8388");
            public static readonly Color TealText = C("#0F766E");
            public static readonly Color TagBg = C("#D5F0EF");
            public static readonly Color Amber = C("#F59E0B");

            public static readonly Color TransitBg = C("#E6F6F7");
            public static readonly Color TransitBorder = C("#9FDCE0");
            public static readonly Color PendingBg = C("#F1F1F0");
            public static readonly Color PendingText = C("#57534E");
            public static readonly Color GreenBg = C("#ECFDF5");
            public static readonly Color GreenBorder = C("#A7F3D0");
            public static readonly Color GreenText = C("#047857");

            public static readonly Font H1 = new Font(Sans, 18F, FontStyle.Bold);
            public static readonly Font H2 = Strong(12F);
            public static readonly Font Subtitle = new Font(Sans, 10.5F);
            public static readonly Font Body = new Font(Sans, 9.5F);
            public static readonly Font BodyBold = Strong(9.5F);
            public static readonly Font VendorName = Strong(11F);
            public static readonly Font Small = new Font(Sans, 8F);
            public static readonly Font Hint = Strong(8.5F);
            public static readonly Font Mono = new Font(MonoName, 8F);
            public static readonly Font MonoBold = new Font(MonoName, 9F, FontStyle.Bold);
            public static readonly Font MonoTag = new Font(MonoName, 8F, FontStyle.Bold);
            public static readonly Font Header = Strong(7.5F);
            public static readonly Font Pill = Strong(8F);
            public static readonly Font PillLarge = Strong(9.5F);
        }

        // ==================================================================
        //  Drawing helpers
        // ==================================================================
        private enum Glyph { None, Plus, CheckCircle }

        private static class Gfx
        {
            public static GraphicsPath RoundRect(RectangleF r, float radius)
            {
                float d = radius * 2f;
                var p = new GraphicsPath();
                p.AddArc(r.X, r.Y, d, d, 180, 90);
                p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
                p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
                p.CloseFigure();
                return p;
            }

            // Square top corners, rounded bottom corners (clips the grid inside its card).
            public static GraphicsPath RoundedBottom(Rectangle r, int radius)
            {
                int d = radius * 2;
                var p = new GraphicsPath();
                p.StartFigure();
                p.AddLine(r.Left, r.Top, r.Right, r.Top);
                p.AddLine(r.Right, r.Top, r.Right, r.Bottom - radius);
                p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
                p.AddLine(r.Right - radius, r.Bottom, r.Left + radius, r.Bottom);
                p.AddArc(r.Left, r.Bottom - d, d, d, 90, 90);
                p.CloseFigure();
                return p;
            }

            public static void DrawGlyph(Graphics g, Glyph glyph, Rectangle r, Color color)
            {
                if (glyph == Glyph.None) return;
                g.SmoothingMode = SmoothingMode.AntiAlias;
                float width = glyph == Glyph.Plus ? 1.8f : 1.3f;
                using (var pen = new Pen(color, width))
                {
                    pen.StartCap = LineCap.Round;
                    pen.EndCap = LineCap.Round;
                    pen.LineJoin = LineJoin.Round;

                    if (glyph == Glyph.Plus)
                    {
                        float cx = r.X + r.Width / 2f, cy = r.Y + r.Height / 2f, h = r.Width * 0.42f;
                        g.DrawLine(pen, cx - h, cy, cx + h, cy);
                        g.DrawLine(pen, cx, cy - h, cx, cy + h);
                    }
                    else
                    {
                        g.DrawEllipse(pen, r.X + 0.5f, r.Y + 0.5f, r.Width - 1, r.Height - 1);
                        g.DrawLines(pen, new[]
                        {
                            new PointF(r.X + r.Width * 0.30f, r.Y + r.Height * 0.52f),
                                    new PointF(r.X + r.Width * 0.45f, r.Y + r.Height * 0.66f),
                                    new PointF(r.X + r.Width * 0.72f, r.Y + r.Height * 0.36f)
                        });
                    }
                }
            }

            public static void DrawPill(Graphics g, Rectangle r, Color fill, Color? border,
                                        string text, Font font, Color fore, Glyph glyph = Glyph.None)
            {
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var rf = new RectangleF(r.X, r.Y, r.Width - 1, r.Height - 1);
                using (var path = RoundRect(rf, rf.Height / 2f))
                using (var brush = new SolidBrush(fill))
                {
                    g.FillPath(brush, path);
                    if (border.HasValue)
                        using (var pen = new Pen(border.Value)) g.DrawPath(pen, path);
                }

                const TextFormatFlags flags = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
                Size ts = TextRenderer.MeasureText(g, text, font, Size.Empty, flags);
                int iconW = glyph == Glyph.None ? 0 : 13;
                int gap = glyph == Glyph.None ? 0 : 6;
                int total = iconW + gap + ts.Width;
                int x = r.X + (r.Width - total) / 2;
                int cy = r.Y + r.Height / 2;

                if (glyph != Glyph.None)
                    DrawGlyph(g, glyph, new Rectangle(x, cy - iconW / 2, iconW, iconW), fore);

                TextRenderer.DrawText(g, text, font, new Point(x + iconW + gap, cy - ts.Height / 2), fore, flags);
            }
        }

        // ==================================================================
        //  Custom controls
        // ==================================================================

        /// <summary>White rounded card with a 1px border.</summary>
        private sealed class RoundedPanel : Panel
        {
            [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public int Radius { get; set; }

            public RoundedPanel()
            {
                Radius = 12;
                BackColor = Color.White;
                DoubleBuffered = true;
                ResizeRedraw = true;
            }

            protected override void OnPaintBackground(PaintEventArgs e) { }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.Clear(Theme.PageBg);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                var r = new RectangleF(0.5f, 0.5f, Width - 2f, Height - 2f);
                using (var path = Gfx.RoundRect(r, Radius))
                using (var fill = new SolidBrush(BackColor))
                using (var pen = new Pen(Theme.Border))
                {
                    g.FillPath(fill, path);
                    g.DrawPath(pen, path);
                }
            }
        }

        /// <summary>Pill-shaped button with optional icon.</summary>
        private sealed class PillButton : Control
        {
            private bool _hover;

            [Browsable(false)]
            [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
            public Glyph Icon { get; set; }

            public PillButton()
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
                Cursor = Cursors.Hand;
                Font = Theme.PillLarge;
                ForeColor = Color.White;
            }

            protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
            protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

            protected override void OnPaint(PaintEventArgs e)
            {
                e.Graphics.Clear(Theme.PageBg);
                Gfx.DrawPill(e.Graphics, new Rectangle(0, 0, Width, Height),
                             _hover ? Theme.TealHover : Theme.Teal, null, Text, Font, ForeColor, Icon);
            }
        }

        /// <summary>Small rounded mono tag such as "SUP-01".</summary>
        private sealed class TagLabel : Control
        {
            public TagLabel(string text)
            {
                SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint |
                ControlStyles.OptimizedDoubleBuffer | ControlStyles.ResizeRedraw, true);
                Text = text;
                Font = Theme.MonoTag;
                Size ts = TextRenderer.MeasureText(text, Font, Size.Empty, TextFormatFlags.NoPadding);
                Size = new Size(ts.Width + 18, 22);
            }

            protected override void OnPaint(PaintEventArgs e)
            {
                var g = e.Graphics;
                g.Clear(Color.White);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (var path = Gfx.RoundRect(new RectangleF(0, 0, Width - 1, Height - 1), 5))
                using (var brush = new SolidBrush(Theme.TagBg))
                g.FillPath(brush, path);

                TextRenderer.DrawText(g, Text, Font, ClientRectangle, Theme.TealText,
                                      TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
            }
        }

        // ==================================================================
        //  Layout constants
        // ==================================================================
        private const int ActionCol = 6;
        private const int PageMargin = 20;
        private const int MinContentWidth = 780;
        private const int CardTop = 108;
        private const int CardHeaderH = 57;       // title row + divider inside the PO card
        private const int GridHeaderH = 46;
        private const int GridRowH = 61;
        private const int CellPad = 12;
        private const int VendorCardH = 232;
        private const int VendorGap = 22;         // horizontal gap between vendor cards
        private const int VendorRowGap = 20;      // vertical gap between vendor rows
        private const int MinVendorCardW = 230;   // below this, fewer cards per row

        // ==================================================================
        //  State
        // ==================================================================
        private readonly List<PoRow> _orders = new List<PoRow>();
        private readonly List<VendorRow> _vendors = new List<VendorRow>();

        private Label _title;
        private Label _subtitle;
        private PillButton _createButton;
        private RoundedPanel _ordersCard;
        private Label _ordersHint;
        private Panel _ordersDivider;
        private Label _ordersEmpty;
        private DataGridView _grid;
        private Label _vendorsTitle;
        private Label _vendorsEmpty;
        private TableLayoutPanel _vendorTable;

        private int _gridHeight = GridHeaderH;
        private int _vendorCols = 4;
        private int _hoverRow = -1;
        private int _hotRow = -1;      // row whose "Mark Received" button is under the mouse
        private bool _inLayout;

        public Suppliers()
        {
            BackColor = Theme.PageBg;
            DoubleBuffered = true;
            AutoScroll = true;

            SuspendLayout();
            BuildHeader();
            BuildOrdersCard();
            BuildVendors();
            ResumeLayout(false);

            LayoutUi();
        }

        protected override void OnResize(EventArgs eventargs)
        {
            base.OnResize(eventargs);
            LayoutUi();
        }

        // ==================================================================
        //  Layout: runs on every resize and whenever data changes
        // ==================================================================
        private void LayoutUi()
        {
            if (_inLayout || _vendorTable == null) return;
            _inLayout = true;
            try
            {
                int ox = DisplayRectangle.X;   // negative when scrolled
                int oy = DisplayRectangle.Y;
                int viewW = Math.Max(ClientSize.Width, MinContentWidth);
                int w = viewW - PageMargin * 2;

                // header
                _title.Location = new Point(ox + PageMargin - 2, oy + 22);
                _subtitle.Location = new Point(ox + PageMargin, oy + 58);
                _createButton.Location = new Point(ox + viewW - PageMargin - _createButton.Width, oy + 30);

                // orders card (grows with the number of rows)
                int rows = _orders.Count;
                _gridHeight = GridHeaderH + rows * GridRowH;
                int cardH = CardHeaderH + _gridHeight + (rows == 0 ? GridRowH : 0) + 1;
                _ordersCard.SetBounds(ox + PageMargin, oy + CardTop, w, cardH);
                LayoutCardChildren();

                // vendors
                int titleY = oy + CardTop + cardH + 33;
                int tableY = titleY + 44;
                _vendorsTitle.Location = new Point(ox + PageMargin - 2, titleY);

                int cols = w >= 4 * MinVendorCardW + 3 * VendorGap ? 4
                : w >= 2 * MinVendorCardW + VendorGap ? 2 : 1;
                if (cols != _vendorCols)
                {
                    _vendorCols = cols;
                    RebuildVendorCards();
                }

                int vRows = _vendors.Count == 0 ? 0 : (_vendors.Count + cols - 1) / cols;
                int tableH = vRows * (VendorCardH + VendorRowGap);
                _vendorTable.SetBounds(ox + PageMargin, tableY, w, tableH);
                _vendorsEmpty.Location = new Point(ox + PageMargin, tableY + 2);

                int bottom = (tableY - oy) + (vRows > 0 ? tableH - VendorRowGap : 28) + 24;
                var want = new Size(MinContentWidth, bottom);
                if (AutoScrollMinSize != want) AutoScrollMinSize = want;
            }
            finally
            {
                _inLayout = false;
            }
        }

        private void LayoutCardChildren()
        {
            int w = _ordersCard.Width;
            _ordersHint.Location = new Point(w - 16 - _ordersHint.GetPreferredSize(Size.Empty).Width, 22);
            _ordersDivider.SetBounds(1, CardHeaderH - 1, Math.Max(0, w - 2), 1);
            _grid.SetBounds(1, CardHeaderH, Math.Max(0, w - 2), _gridHeight);

            var es = _ordersEmpty.GetPreferredSize(Size.Empty);
            _ordersEmpty.Location = new Point((w - es.Width) / 2,
                                              CardHeaderH + _gridHeight + (GridRowH - es.Height) / 2);
        }

        // ==================================================================
        //  Header
        // ==================================================================
        private void BuildHeader()
        {
            _title = new Label
            {
                Text = "Suppliers & Purchase Orders",
                Font = Theme.H1,
                ForeColor = Theme.Ink,
                BackColor = Theme.PageBg,
                AutoSize = true,
                UseMnemonic = false     // otherwise "&" is swallowed
            };

            _subtitle = new Label
            {
                Text = "Track incoming shipments, lot batch updates, and vendor ratings.",
                Font = Theme.Subtitle,
                ForeColor = Theme.Muted,
                BackColor = Theme.PageBg,
                AutoSize = true,
                UseMnemonic = false
            };

            _createButton = new PillButton
            {
                Text = "Create Purchase Order",
                Icon = Glyph.Plus,
                Size = new Size(212, 42)
            };
            _createButton.Click += (s, e) =>
            {
                var h = CreatePurchaseOrderRequested;
                if (h != null) h(this, EventArgs.Empty);
            };

                Controls.Add(_title);
                Controls.Add(_subtitle);
                Controls.Add(_createButton);
        }

        // ==================================================================
        //  Inbound PO card
        // ==================================================================
        private void BuildOrdersCard()
        {
            _ordersCard = new RoundedPanel();

            _ordersCard.Controls.Add(new Label
            {
                Text = "Inbound Purchase Orders",
                Font = Theme.H2,
                ForeColor = Theme.Ink,
                BackColor = Color.White,
                AutoSize = true,
                UseMnemonic = false,
                Location = new Point(14, 15)
            });

            _ordersHint = new Label
            {
                Text = "Fulfilling PO updates stock & lot numbers",
                Font = Theme.Hint,
                ForeColor = Theme.Faint,
                BackColor = Color.White,
                AutoSize = true,
                UseMnemonic = false
            };
            _ordersCard.Controls.Add(_ordersHint);

            _ordersDivider = new Panel { BackColor = Theme.Border };
            _ordersCard.Controls.Add(_ordersDivider);

            _ordersEmpty = new Label
            {
                Text = "No purchase orders yet.",
                Font = Theme.Body,
                ForeColor = Theme.Faint,
                BackColor = Color.White,
                AutoSize = true,
                UseMnemonic = false
            };
            _ordersCard.Controls.Add(_ordersEmpty);

            BuildGrid();
            _ordersCard.Controls.Add(_grid);
            Controls.Add(_ordersCard);
        }

        private void BuildGrid()
        {
            _grid = new DataGridView
            {
                AllowUserToAddRows = false,
                AllowUserToDeleteRows = false,
                AllowUserToResizeRows = false,
                AllowUserToResizeColumns = false,
                ReadOnly = true,
                MultiSelect = false,
                RowHeadersVisible = false,
                BorderStyle = BorderStyle.None,
                CellBorderStyle = DataGridViewCellBorderStyle.None,
                BackgroundColor = Color.White,
                EnableHeadersVisualStyles = false,
                ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None,
                ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing,
                ColumnHeadersHeight = GridHeaderH,
                AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill,
                SelectionMode = DataGridViewSelectionMode.FullRowSelect,
                ScrollBars = ScrollBars.None
            };

            typeof(DataGridView)
            .GetProperty("DoubleBuffered", BindingFlags.Instance | BindingFlags.NonPublic)
            ?.SetValue(_grid, true);

            var hs = _grid.ColumnHeadersDefaultCellStyle;
            hs.BackColor = Theme.HeaderBg;
            hs.ForeColor = Theme.Muted;
            hs.Font = Theme.Header;
            hs.Alignment = DataGridViewContentAlignment.MiddleLeft;
            hs.Padding = new Padding(CellPad, 0, CellPad, 0);
            hs.WrapMode = DataGridViewTriState.False;
            hs.SelectionBackColor = Theme.HeaderBg;
            hs.SelectionForeColor = Theme.Muted;

            _grid.DefaultCellStyle.SelectionBackColor = Color.White;
            _grid.DefaultCellStyle.SelectionForeColor = Theme.Ink;
            _grid.RowTemplate.Height = GridRowH;

            string[] heads = { "PO NUMBER", "SUPPLIER", "ITEM & BATCH LOT", "QTY & COST", "DATES", "STATUS", "ACTION" };
            float[] weights = { 11, 15, 21, 10, 15, 10, 18 };
            int[] mins = { 98, 112, 152, 80, 128, 94, 140 };   // keeps columns from colliding when narrow
            for (int i = 0; i < heads.Length; i++)
            {
                _grid.Columns.Add(new DataGridViewTextBoxColumn
                {
                    HeaderText = heads[i],
                    FillWeight = weights[i],
                    MinimumWidth = mins[i],
                    SortMode = DataGridViewColumnSortMode.NotSortable
                });
            }
            _grid.Columns[ActionCol].HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;

            _grid.CellPainting += Grid_CellPainting;
            _grid.CellMouseEnter += (s, e) =>
            {
                if (e.RowIndex >= 0 && e.RowIndex != _hoverRow) { _hoverRow = e.RowIndex; _grid.Invalidate(); }
            };
            _grid.MouseLeave += (s, e) =>
            {
                _hoverRow = -1; _hotRow = -1; _grid.Cursor = Cursors.Default; _grid.Invalidate();
            };
            _grid.CellMouseMove += Grid_CellMouseMove;
            _grid.CellMouseClick += Grid_CellMouseClick;
            _grid.SizeChanged += (s, e) => ApplyGridRegion();
        }

        // Round the grid's bottom corners so it doesn't poke out of the card.
        private void ApplyGridRegion()
        {
            var old = _grid.Region;
            if (_orders.Count == 0 || _grid.Width < 30 || _grid.Height < 30)
            {
                _grid.Region = null;
            }
            else
            {
                using (var path = Gfx.RoundedBottom(new Rectangle(0, 0, _grid.Width, _grid.Height), 11))
                _grid.Region = new Region(path);
            }
            if (old != null) old.Dispose();
        }

        // ==================================================================
        //  Grid painting
        // ==================================================================
        private static Rectangle ActionRect(Rectangle cell)
        {
            int w = Math.Min(128, Math.Max(40, cell.Width - CellPad * 2));
            return new Rectangle(cell.Right - CellPad - w, cell.Y + (cell.Height - 28) / 2, w, 28);
        }

        private Rectangle CellLocal(int col, int row)
        {
            return new Rectangle(0, 0, _grid.Columns[col].Width, _grid.Rows[row].Height);
        }

        private const TextFormatFlags TextFlags =
        TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis | TextFormatFlags.Left;

        private static void OneLine(Graphics g, Rectangle b, int x, int w, string text, Font font, Color color)
        {
            TextRenderer.DrawText(g, text ?? "", font, new Rectangle(x, b.Y, w, b.Height), color,
                                  TextFlags | TextFormatFlags.VerticalCenter);
        }

        private static void TwoLines(Graphics g, Rectangle b, int x, int w,
                                     string t1, Font f1, Color c1, string t2, Font f2, Color c2)
        {
            int h1 = f1.Height, h2 = f2.Height, gap = 1;
            int top = b.Y + (b.Height - (h1 + gap + h2)) / 2;
            TextRenderer.DrawText(g, t1 ?? "", f1, new Rectangle(x, top, w, h1), c1, TextFlags);
            TextRenderer.DrawText(g, t2 ?? "", f2, new Rectangle(x, top + h1 + gap, w, h2), c2, TextFlags);
        }

        private void Grid_CellPainting(object sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0) return;   // headers paint normally
            if (e.RowIndex >= _orders.Count) return;

            var po = _orders[e.RowIndex];
            var g = e.Graphics;
            var b = e.CellBounds;
            var us = CultureInfo.GetCultureInfo("en-US");

            using (var bg = new SolidBrush(e.RowIndex == _hoverRow ? Theme.RowHover : Color.White))
            g.FillRectangle(bg, b);

            using (var line = new Pen(Theme.RowLine))
            {
                if (e.RowIndex < _orders.Count - 1)
                    g.DrawLine(line, b.Left, b.Bottom - 1, b.Right, b.Bottom - 1);
                if (e.RowIndex == 0)
                    g.DrawLine(line, b.Left, b.Top, b.Right, b.Top);
            }

            int x = b.X + CellPad;
            int w = Math.Max(0, b.Width - CellPad * 2);

            switch (e.ColumnIndex)
            {
                case 0:
                    OneLine(g, b, x, w, po.Number, Theme.MonoBold, Theme.TealText);
                    break;
                case 1:
                    OneLine(g, b, x, w, po.Supplier, Theme.BodyBold, Theme.Ink);
                    break;
                case 2:
                    TwoLines(g, b, x, w, po.Item, Theme.BodyBold, Theme.Ink,
                             "Lot: " + po.Lot, Theme.Mono, Theme.Faint);
                    break;
                case 3:
                    TwoLines(g, b, x, w, po.Qty + " units", Theme.BodyBold, Theme.Ink,
                             po.Cost.ToString("C2", us), Theme.Small, Theme.Faint);
                    break;
                case 4:
                    TwoLines(g, b, x, w, "Ordered: " + po.Ordered.ToString("yyyy-MM-dd"), Theme.Body, Theme.BodyText,
                             "Expected: " + po.Expected.ToString("yyyy-MM-dd"), Theme.Small, Theme.Faint);
                    break;
                case 5:
                    PaintStatus(g, b, x, po.Status);
                    break;
                case ActionCol:
                    PaintAction(g, b, po, e.RowIndex == _hotRow);
                    break;
            }

            e.Handled = true;
        }

        private static void PaintStatus(Graphics g, Rectangle b, int x, PoStatus status)
        {
            string text;
            Color fill, fore;
            Color? border;

            switch (status)
            {
                case PoStatus.InTransit:
                    text = "In Transit"; fill = Theme.TransitBg; border = Theme.TransitBorder; fore = Theme.TealText; break;
                case PoStatus.Received:
                    text = "Received"; fill = Theme.GreenBg; border = Theme.GreenBorder; fore = Theme.GreenText; break;
                default:
                    text = "Pending"; fill = Theme.PendingBg; border = null; fore = Theme.PendingText; break;
            }

            Size ts = TextRenderer.MeasureText(g, text, Theme.Pill, Size.Empty,
                                               TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix);
            var r = new Rectangle(x, b.Y + (b.Height - 24) / 2, ts.Width + 24, 24);
            Gfx.DrawPill(g, r, fill, border, text, Theme.Pill, fore);
        }

        private static void PaintAction(Graphics g, Rectangle b, PoRow po, bool hot)
        {
            if (po.Status != PoStatus.Received)
            {
                Gfx.DrawPill(g, ActionRect(b), hot ? Theme.TealHover : Theme.Teal, null,
                             "Mark Received", Theme.Pill, Color.White, Glyph.CheckCircle);
                return;
            }

            const TextFormatFlags f = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
            string text = "Received & Synchronized";
            Size ts = TextRenderer.MeasureText(g, text, Theme.Pill, Size.Empty, f);

            // Not enough room? Use the short label instead of overlapping neighbouring cells.
            if (13 + 6 + ts.Width > b.Width - CellPad * 2)
            {
                text = "Synchronized";
                ts = TextRenderer.MeasureText(g, text, Theme.Pill, Size.Empty, f);
            }

            int total = 13 + 6 + ts.Width;
            int sx = b.Right - CellPad - total;
            int cy = b.Y + b.Height / 2;
            Gfx.DrawGlyph(g, Glyph.CheckCircle, new Rectangle(sx, cy - 6, 13, 13), Theme.GreenText);
            TextRenderer.DrawText(g, text, Theme.Pill, new Point(sx + 19, cy - ts.Height / 2), Theme.GreenText, f);
        }

        // ==================================================================
        //  Grid interaction
        // ==================================================================
        private bool IsOverButton(DataGridViewCellMouseEventArgs e)
        {
            return e.RowIndex >= 0
            && e.RowIndex < _orders.Count
            && e.ColumnIndex == ActionCol
            && _orders[e.RowIndex].Status != PoStatus.Received
            && ActionRect(CellLocal(e.ColumnIndex, e.RowIndex)).Contains(e.Location);
        }

        private void Grid_CellMouseMove(object sender, DataGridViewCellMouseEventArgs e)
        {
            int hot = IsOverButton(e) ? e.RowIndex : -1;
            if (hot == _hotRow) return;
            _hotRow = hot;
            _grid.Cursor = hot >= 0 ? Cursors.Hand : Cursors.Default;
            _grid.Invalidate();
        }

        private void Grid_CellMouseClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (!IsOverButton(e)) return;

            // The panel doesn't touch the data: the controller updates the DB, then calls
            // SetPurchaseOrderStatus(...) to flip the row.
            var h = MarkReceivedRequested;
            if (h != null) h(this, new PoEventArgs(_orders[e.RowIndex]));
        }

        // ==================================================================
        //  Vendor cards
        // ==================================================================
        private void BuildVendors()
        {
            _vendorsTitle = new Label
            {
                Text = "Approved Wellness Vendors",
                Font = Theme.H2,
                ForeColor = Theme.Ink,
                BackColor = Theme.PageBg,
                AutoSize = true,
                UseMnemonic = false
            };
            Controls.Add(_vendorsTitle);

            _vendorsEmpty = new Label
            {
                Text = "No approved vendors yet.",
                Font = Theme.Body,
                ForeColor = Theme.Faint,
                BackColor = Theme.PageBg,
                AutoSize = true,
                UseMnemonic = false
            };
            Controls.Add(_vendorsEmpty);

            _vendorTable = new TableLayoutPanel { BackColor = Theme.PageBg };
            Controls.Add(_vendorTable);
        }

        private void RebuildVendorCards()
        {
            int cols = Math.Max(1, _vendorCols);
            int rows = _vendors.Count == 0 ? 0 : (_vendors.Count + cols - 1) / cols;

            _vendorTable.SuspendLayout();

            for (int i = _vendorTable.Controls.Count - 1; i >= 0; i--)
                _vendorTable.Controls[i].Dispose();
            _vendorTable.ColumnStyles.Clear();
            _vendorTable.RowStyles.Clear();

            _vendorTable.ColumnCount = cols;
            _vendorTable.RowCount = Math.Max(rows, 1);
            for (int c = 0; c < cols; c++)
                _vendorTable.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f / cols));
            for (int r = 0; r < Math.Max(rows, 1); r++)
                _vendorTable.RowStyles.Add(new RowStyle(SizeType.Absolute, VendorCardH + VendorRowGap));

            for (int i = 0; i < _vendors.Count; i++)
            {
                var card = BuildVendorCard(_vendors[i]);
                bool lastInRow = (i % cols) == cols - 1;
                card.Dock = DockStyle.Fill;
                card.Margin = new Padding(0, 0, lastInRow ? 0 : VendorGap, VendorRowGap);
                _vendorTable.Controls.Add(card, i % cols, i / cols);
            }

            _vendorTable.ResumeLayout(true);
        }

        private static Label MakeLabel(string text, Font font, Color color, int x, int y)
        {
            return new Label
            {
                Text = text ?? "",
                Font = font,
                ForeColor = color,
                BackColor = Color.White,
                AutoSize = true,
                UseMnemonic = false,
                Location = new Point(x, y)
            };
        }

        private RoundedPanel BuildVendorCard(VendorRow v)
        {
            const int pad = 20;
            var card = new RoundedPanel();

            var tag = new TagLabel(v.Code ?? "") { Location = new Point(pad, 19) };
            var rating = MakeLabel("\u2605 " + v.Rating.ToString("0.0", CultureInfo.InvariantCulture),
                                   Theme.BodyBold, Theme.Amber, 0, 20);

            var name = MakeLabel(v.Name, Theme.VendorName, Theme.Ink, pad - 2, 46);
            var contact = MakeLabel(v.Contact, Theme.Body, Theme.Muted, pad - 2, 72);
            var lead = MakeLabel("Lead Time: " + v.LeadTime, Theme.Body, Theme.BodyText, pad - 2, 100);
            var email = MakeLabel(v.Email, Theme.Body, Theme.Faint, pad - 2, 122);
            var phone = MakeLabel(v.Phone, Theme.Body, Theme.Faint, pad - 2, 144);

            var sep = new Panel { BackColor = Theme.RowLine, Location = new Point(pad, 180), Height = 1 };

            var city = MakeLabel(v.City, Theme.Small, Theme.Faint, pad - 2, 196);
            var link = MakeLabel("New PO \u2192", Theme.BodyBold, Theme.TealText, 0, 193);
            link.Cursor = Cursors.Hand;
            link.Click += (s, e) =>
            {
                var h = NewPurchaseOrderForVendorRequested;
                if (h != null) h(this, new VendorEventArgs(v));
            };

                card.Controls.AddRange(new Control[] { tag, rating, name, contact, lead, email, phone, sep, city, link });

                // Keep the right-aligned items pinned to the card's right edge.
                card.Resize += (s, e) =>
                {
                    rating.Left = card.Width - pad - rating.GetPreferredSize(Size.Empty).Width;
                    link.Left = card.Width - pad - link.GetPreferredSize(Size.Empty).Width;
                    sep.Width = Math.Max(0, card.Width - pad * 2);
                };

                return card;
        }
    }
}
