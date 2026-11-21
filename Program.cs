using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Text;
using System.Windows.Forms;

namespace management_system_test
{
    internal static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new MainInventoryForm());
        }
    }

    public static class BrandTheme
    {
        public static readonly Color SidebarBg = ColorTranslator.FromHtml("#062B30");
        public static readonly Color SidebarHover = ColorTranslator.FromHtml("#0B3D44");
        public static readonly Color SidebarMuted = ColorTranslator.FromHtml("#94B8BE");
        public static readonly Color PrimaryTeal = ColorTranslator.FromHtml("#0B96A0");
        public static readonly Color PrimaryHover = ColorTranslator.FromHtml("#097D85");
        public static readonly Color SoftTealBg = ColorTranslator.FromHtml("#E6F6F7");
        public static readonly Color SoftTealText = ColorTranslator.FromHtml("#07666D");
        public static readonly Color CanvasStone = ColorTranslator.FromHtml("#F5F5F4");
        public static readonly Color CardWhite = Color.White;
        public static readonly Color BorderSubtle = ColorTranslator.FromHtml("#E7E5E4");
        public static readonly Color TextDark = ColorTranslator.FromHtml("#1C1917");
        public static readonly Color TextMuted = ColorTranslator.FromHtml("#57534E");
        public static readonly Color DangerBg = ColorTranslator.FromHtml("#FEE2E2");
        public static readonly Color DangerText = ColorTranslator.FromHtml("#B91C1C");
        public static readonly Color WarningBg = ColorTranslator.FromHtml("#FEF3C7");
        public static readonly Color WarningText = ColorTranslator.FromHtml("#B45309");
        public static readonly Color SuccessBg = ColorTranslator.FromHtml("#DCFCE7");
        public static readonly Color SuccessText = ColorTranslator.FromHtml("#15803D");

        public static Font HeaderFont { get { return new Font("Segoe UI", 16f, FontStyle.Bold); } }
        public static Font SubHeadFont { get { return new Font("Segoe UI", 11f, FontStyle.Bold); } }
        public static Font BodyFont { get { return new Font("Segoe UI", 9.5f, FontStyle.Regular); } }
        public static Font BoldBodyFont { get { return new Font("Segoe UI", 9.5f, FontStyle.Bold); } }
        public static Font MonoFont { get { return new Font("Consolas", 9.5f, FontStyle.Regular); } }
    }

    public class ProductItem
    {
        public string Sku;
        public string Name;
        public string DiscreetName;
        public string Category;
        public string Material;
        public string WaterproofRating;
        public string LotNumber;
        public DateTime? ExpiryDate;
        public int Stock;
        public int ReorderPoint;
        public decimal UnitCost;
        public decimal RetailPrice;

        public string GetStatus()
        {
            if (Stock == 0) return "Out of Stock";
            if (ExpiryDate.HasValue && ExpiryDate.Value <= DateTime.Today.AddDays(45)) return "Expiring Soon";
            if (Stock <= ReorderPoint) return "Low Stock";
            return "In Stock";
        }
    }

    public class PurchaseOrder
    {
        public string PoNumber;
        public string Supplier;
        public string TargetSku;
        public string ProductSummary;
        public int Quantity;
        public decimal TotalCost;
        public string Status;
        public DateTime ExpectedDate;
    }

    public class AuditLogEntry
    {
        public DateTime Timestamp;
        public string Sku;
        public string ProductName;
        public string ActionType;
        public int QuantityDelta;
        public int BalanceAfter;
        public string Notes;
    }

    public class MainInventoryForm : Form
    {
        private List<ProductItem> _products;
        private List<PurchaseOrder> _purchaseOrders;
        private List<AuditLogEntry> _auditLogs;

        private bool _discreetMode = false;
        private string _activeTab = "Dashboard";

        private Panel _sidebarPanel;
        private Panel _headerPanel;
        private Panel _contentHost;
        private Label _lblPageTitle;
        private CheckBox _chkDiscreetMode;

        private Dictionary<string, Button> _navButtons = new Dictionary<string, Button>();

        private DataGridView _dgvCatalog;
        private TextBox _txtSearch;
        private ComboBox _cmbCategoryFilter;
        private DataGridView _dgvOrders;
        private DataGridView _dgvAudit;

        public MainInventoryForm()
        {
            SeedDemoData();
            InitializeWindow();
            BuildSidebar();
            BuildHeader();
            SwitchView("Dashboard");
        }

        private void InitializeWindow()
        {
            this.Text = "AURA WELLNESS - Intimate Inventory & FEFO Management";
            this.Size = new Size(1340, 840);
            this.MinimumSize = new Size(1100, 700);
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = BrandTheme.CanvasStone;
            this.Font = BrandTheme.BodyFont;

            _contentHost = new Panel();
            _contentHost.Dock = DockStyle.Fill;
            _contentHost.BackColor = BrandTheme.CanvasStone;
            _contentHost.Padding = new Padding(24);
            this.Controls.Add(_contentHost);
        }

        private void BuildSidebar()
        {
            _sidebarPanel = new Panel();
            _sidebarPanel.Dock = DockStyle.Left;
            _sidebarPanel.Width = 245;
            _sidebarPanel.BackColor = BrandTheme.SidebarBg;
            _sidebarPanel.Padding = new Padding(16, 22, 16, 22);

            Label lblBrand = new Label();
            lblBrand.Text = "AURA IMS";
            lblBrand.ForeColor = Color.White;
            lblBrand.Font = new Font("Segoe UI", 14f, FontStyle.Bold);
            lblBrand.Dock = DockStyle.Top;
            lblBrand.Height = 36;

            Label lblSubBrand = new Label();
            lblSubBrand.Text = "BOUTIQUE WELLNESS SUITE";
            lblSubBrand.ForeColor = BrandTheme.PrimaryTeal;
            lblSubBrand.Font = new Font("Segoe UI", 7.5f, FontStyle.Bold);
            lblSubBrand.Dock = DockStyle.Top;
            lblSubBrand.Height = 28;

            _sidebarPanel.Controls.Add(lblSubBrand);
            _sidebarPanel.Controls.Add(lblBrand);

            string[] navItems = new string[] { "Audit Log", "Purchase Orders", "Inventory Catalog", "Dashboard" };
            for (int i = 0; i < navItems.Length; i++)
            {
                string currentItem = navItems[i];
                Button btn = new Button();
                btn.Text = "   " + currentItem;
                btn.Tag = currentItem;
                btn.Dock = DockStyle.Top;
                btn.Height = 44;
                btn.FlatStyle = FlatStyle.Flat;
                btn.TextAlign = ContentAlignment.MiddleLeft;
                btn.Font = BrandTheme.BoldBodyFont;
                btn.ForeColor = BrandTheme.SidebarMuted;
                btn.BackColor = BrandTheme.SidebarBg;
                btn.Cursor = Cursors.Hand;
                btn.Margin = new Padding(0, 4, 0, 4);
                btn.FlatAppearance.BorderSize = 0;
                btn.FlatAppearance.MouseOverBackColor = BrandTheme.SidebarHover;
                btn.Click += delegate (object s, EventArgs e) { SwitchView(currentItem); };

                _navButtons[currentItem] = btn;
                _sidebarPanel.Controls.Add(btn);
            }

            Label lblFooter = new Label();
            lblFooter.Text = "FEFO Lot Tracking Active\nTheme: Cerulean #0B96A0";
            lblFooter.ForeColor = BrandTheme.SidebarMuted;
            lblFooter.Font = new Font("Segoe UI", 8f);
            lblFooter.Dock = DockStyle.Bottom;
            lblFooter.Height = 38;
            _sidebarPanel.Controls.Add(lblFooter);

            this.Controls.Add(_sidebarPanel);
        }

        private void BuildHeader()
        {
            _headerPanel = new Panel();
            _headerPanel.Dock = DockStyle.Top;
            _headerPanel.Height = 68;
            _headerPanel.BackColor = BrandTheme.CardWhite;
            _headerPanel.Padding = new Padding(24, 14, 24, 14);
            _headerPanel.Paint += delegate (object s, PaintEventArgs e)
            {
                using (Pen pen = new Pen(BrandTheme.BorderSubtle, 1))
                {
                    e.Graphics.DrawLine(pen, 0, _headerPanel.Height - 1, _headerPanel.Width, _headerPanel.Height - 1);
                }
            };

            _lblPageTitle = new Label();
            _lblPageTitle.Text = "Dashboard Overview";
            _lblPageTitle.Font = BrandTheme.HeaderFont;
            _lblPageTitle.ForeColor = BrandTheme.TextDark;
            _lblPageTitle.Dock = DockStyle.Left;
            _lblPageTitle.AutoSize = true;

            _chkDiscreetMode = new CheckBox();
            _chkDiscreetMode.Appearance = Appearance.Button;
            _chkDiscreetMode.Text = "Discreet Counter Mode: OFF";
            _chkDiscreetMode.Dock = DockStyle.Right;
            _chkDiscreetMode.Width = 235;
            _chkDiscreetMode.FlatStyle = FlatStyle.Flat;
            _chkDiscreetMode.TextAlign = ContentAlignment.MiddleCenter;
            _chkDiscreetMode.Font = BrandTheme.BoldBodyFont;
            _chkDiscreetMode.BackColor = BrandTheme.CanvasStone;
            _chkDiscreetMode.ForeColor = BrandTheme.TextDark;
            _chkDiscreetMode.Cursor = Cursors.Hand;
            _chkDiscreetMode.FlatAppearance.BorderColor = BrandTheme.BorderSubtle;
            _chkDiscreetMode.CheckedChanged += delegate (object s, EventArgs e)
            {
                _discreetMode = _chkDiscreetMode.Checked;
                _chkDiscreetMode.Text = _discreetMode
                    ? "Discreet Counter Mode: ON"
                    : "Discreet Counter Mode: OFF";
                _chkDiscreetMode.BackColor = _discreetMode ? BrandTheme.SoftTealBg : BrandTheme.CanvasStone;
                _chkDiscreetMode.ForeColor = _discreetMode ? BrandTheme.SoftTealText : BrandTheme.TextDark;
                RefreshActiveView();
            };

            _headerPanel.Controls.Add(_chkDiscreetMode);
            _headerPanel.Controls.Add(_lblPageTitle);
            this.Controls.Add(_headerPanel);
        }

        private void SwitchView(string tabName)
        {
            _activeTab = tabName;
            _lblPageTitle.Text = tabName;

            foreach (KeyValuePair<string, Button> kvp in _navButtons)
            {
                bool isActive = (kvp.Key == tabName);
                kvp.Value.BackColor = isActive ? BrandTheme.PrimaryTeal : BrandTheme.SidebarBg;
                kvp.Value.ForeColor = isActive ? Color.White : BrandTheme.SidebarMuted;
            }

            RefreshActiveView();
        }

        private void RefreshActiveView()
        {
            _contentHost.Controls.Clear();

            if (_activeTab == "Dashboard") RenderDashboardView();
            else if (_activeTab == "Inventory Catalog") RenderCatalogView();
            else if (_activeTab == "Purchase Orders") RenderPurchaseOrdersView();
            else if (_activeTab == "Audit Log") RenderAuditLogView();
        }

        private void RenderDashboardView()
        {
            TableLayoutPanel layout = new TableLayoutPanel();
            layout.Dock = DockStyle.Fill;
            layout.RowCount = 2;
            layout.ColumnCount = 1;
            layout.BackColor = BrandTheme.CanvasStone;
            layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 130));
            layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100));

            TableLayoutPanel kpiGrid = new TableLayoutPanel();
            kpiGrid.Dock = DockStyle.Fill;
            kpiGrid.ColumnCount = 4;
            kpiGrid.RowCount = 1;
            kpiGrid.Margin = new Padding(0, 0, 0, 16);
            for (int i = 0; i < 4; i++) kpiGrid.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25));

            decimal totalVal = 0;
            int totalUnits = 0;
            int lowStockCount = 0;
            int expiringCount = 0;

            for (int i = 0; i < _products.Count; i++)
            {
                ProductItem p = _products[i];
                totalVal += p.Stock * p.UnitCost;
                totalUnits += p.Stock;
                if (p.Stock <= p.ReorderPoint) lowStockCount++;
                if (p.ExpiryDate.HasValue && p.ExpiryDate.Value <= DateTime.Today.AddDays(45)) expiringCount++;
            }

            kpiGrid.Controls.Add(CreateKpiCard("TOTAL INVENTORY VALUE", string.Format("${0:N2}", totalVal), "Wholesale asset cost", BrandTheme.PrimaryTeal), 0, 0);
            kpiGrid.Controls.Add(CreateKpiCard("ACTIVE UNITS ON HAND", string.Format("{0:N0} units", totalUnits), string.Format("Across {0} wellness SKUs", _products.Count), BrandTheme.TextDark), 1, 0);
            kpiGrid.Controls.Add(CreateKpiCard("LOW / OUT OF STOCK", string.Format("{0} items", lowStockCount), "Below safety threshold", lowStockCount > 0 ? BrandTheme.DangerText : BrandTheme.SuccessText), 2, 0);
            kpiGrid.Controls.Add(CreateKpiCard("FEFO EXPIRY WATCH", string.Format("{0} batches", expiringCount), "Expiring within 45 days", expiringCount > 0 ? BrandTheme.WarningText : BrandTheme.SuccessText), 3, 0);

            layout.Controls.Add(kpiGrid, 0, 0);

            Panel alertCard = CreateCardContainer();
            Label lblAlertHeader = new Label();
            lblAlertHeader.Text = "Priority Restock & FEFO Batch Watchlist";
            lblAlertHeader.Font = BrandTheme.SubHeadFont;
            lblAlertHeader.ForeColor = BrandTheme.TextDark;
            lblAlertHeader.Dock = DockStyle.Top;
            lblAlertHeader.Height = 36;

            DataGridView dgvAlerts = CreateStyledGrid();
            dgvAlerts.Columns.Add("Sku", "SKU");
            dgvAlerts.Columns.Add("Name", "Product Description");
            dgvAlerts.Columns.Add("Lot", "Lot / Batch #");
            dgvAlerts.Columns.Add("Expiry", "FEFO Expiry");
            dgvAlerts.Columns.Add("Stock", "On Hand");
            dgvAlerts.Columns.Add("Status", "Alert Status");

            for (int i = 0; i < _products.Count; i++)
            {
                ProductItem p = _products[i];
                string st = p.GetStatus();
                if (st != "In Stock")
                {
                    dgvAlerts.Rows.Add(
                        p.Sku,
                        _discreetMode ? p.DiscreetName : p.Name,
                        p.LotNumber,
                        p.ExpiryDate.HasValue ? p.ExpiryDate.Value.ToString("yyyy-MM-dd") : "Non-Perishable",
                        string.Format("{0} / {1} min", p.Stock, p.ReorderPoint),
                        st
                    );
                }
            }

            ApplyStatusBadgeFormatting(dgvAlerts, 5);
            alertCard.Controls.Add(dgvAlerts);
            alertCard.Controls.Add(lblAlertHeader);
            layout.Controls.Add(alertCard, 0, 1);

            _contentHost.Controls.Add(layout);
        }

        private void RenderCatalogView()
        {
            Panel card = CreateCardContainer();
            Panel toolbar = new Panel();
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 48;
            toolbar.Padding = new Padding(0, 0, 0, 10);

            Label lblSearchLabel = new Label();
            lblSearchLabel.Text = "Filter:";
            lblSearchLabel.AutoSize = true;
            lblSearchLabel.Location = new Point(0, 8);
            lblSearchLabel.ForeColor = BrandTheme.TextMuted;

            _txtSearch = new TextBox();
            _txtSearch.Width = 270;
            _txtSearch.Location = new Point(45, 4);
            _txtSearch.Font = BrandTheme.BodyFont;
            _txtSearch.TextChanged += delegate (object s, EventArgs e) { PopulateCatalogGrid(); };

            _cmbCategoryFilter = new ComboBox();
            _cmbCategoryFilter.DropDownStyle = ComboBoxStyle.DropDownList;
            _cmbCategoryFilter.Width = 210;
            _cmbCategoryFilter.Location = new Point(325, 4);
            _cmbCategoryFilter.Font = BrandTheme.BodyFont;
            _cmbCategoryFilter.Items.AddRange(new object[]
            {
                "All Categories",
                "Personal Massagers",
                "Intimate Wellness",
                "Lubricants & Care",
                "Couples & Accessories"
            });
            _cmbCategoryFilter.SelectedIndex = 0;
            _cmbCategoryFilter.SelectedIndexChanged += delegate (object s, EventArgs e) { PopulateCatalogGrid(); };

            Button btnAddProduct = CreatePrimaryButton("+ Add New SKU", 145);
            btnAddProduct.Dock = DockStyle.Right;
            btnAddProduct.Click += delegate (object s, EventArgs e) { ShowAddProductDialog(); };

            Button btnAdjustStock = CreateSecondaryButton("Adjust Selected Stock", 175);
            btnAdjustStock.Dock = DockStyle.Right;
            btnAdjustStock.Click += delegate (object s, EventArgs e) { AdjustSelectedStock(); };

            toolbar.Controls.Add(lblSearchLabel);
            toolbar.Controls.Add(_txtSearch);
            toolbar.Controls.Add(_cmbCategoryFilter);
            toolbar.Controls.Add(btnAdjustStock);
            toolbar.Controls.Add(btnAddProduct);

            _dgvCatalog = CreateStyledGrid();
            _dgvCatalog.Columns.Add("Sku", "SKU");
            _dgvCatalog.Columns.Add("Name", "Product Name");
            _dgvCatalog.Columns.Add("Category", "Category");
            _dgvCatalog.Columns.Add("Material", "Body-Safe Material & Rating");
            _dgvCatalog.Columns.Add("Lot", "Batch / Expiry");
            _dgvCatalog.Columns.Add("Stock", "Qty");
            _dgvCatalog.Columns.Add("Price", "Retail");
            _dgvCatalog.Columns.Add("Status", "Status");

            _dgvCatalog.Columns["Sku"].Width = 115;
            _dgvCatalog.Columns["Stock"].Width = 65;
            _dgvCatalog.Columns["Price"].Width = 85;
            _dgvCatalog.Columns["Status"].Width = 115;

            ApplyStatusBadgeFormatting(_dgvCatalog, 7);
            PopulateCatalogGrid();

            card.Controls.Add(_dgvCatalog);
            card.Controls.Add(toolbar);
            _contentHost.Controls.Add(card);
        }

        private void PopulateCatalogGrid()
        {
            if (_dgvCatalog == null) return;
            _dgvCatalog.Rows.Clear();

            string query = (_txtSearch != null && _txtSearch.Text != null) ? _txtSearch.Text.Trim().ToLower() : "";
            string cat = (_cmbCategoryFilter != null && _cmbCategoryFilter.SelectedItem != null) ? _cmbCategoryFilter.SelectedItem.ToString() : "All Categories";

            for (int i = 0; i < _products.Count; i++)
            {
                ProductItem p = _products[i];
                bool catMatch = (cat == "All Categories" || p.Category == cat);
                bool textMatch = string.IsNullOrEmpty(query) ||
                    p.Sku.ToLower().Contains(query) ||
                    p.Name.ToLower().Contains(query) ||
                    p.Material.ToLower().Contains(query) ||
                    p.LotNumber.ToLower().Contains(query);

                if (catMatch && textMatch)
                {
                    string displayName = _discreetMode ? p.DiscreetName : p.Name;
                    string lotDisplay = p.ExpiryDate.HasValue
                        ? string.Format("{0} (Exp {1:MM/yy})", p.LotNumber, p.ExpiryDate.Value)
                        : string.Format("{0} (No Exp)", p.LotNumber);

                    _dgvCatalog.Rows.Add(
                        p.Sku,
                        displayName,
                        p.Category,
                        string.Format("{0} - {1}", p.Material, p.WaterproofRating),
                        lotDisplay,
                        p.Stock,
                        string.Format("${0:N2}", p.RetailPrice),
                        p.GetStatus()
                    );
                }
            }
        }

        private void RenderPurchaseOrdersView()
        {
            Panel card = CreateCardContainer();
            Panel toolbar = new Panel();
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 46;
            toolbar.Padding = new Padding(0, 0, 0, 10);

            Label lblHint = new Label();
            lblHint.Text = "Select an 'Ordered' PO and click 'Mark Selected PO Received' to automatically increment inventory.";
            lblHint.ForeColor = BrandTheme.TextMuted;
            lblHint.Dock = DockStyle.Left;
            lblHint.AutoSize = true;

            Button btnReceive = CreatePrimaryButton("Mark Selected PO Received", 220);
            btnReceive.Dock = DockStyle.Right;
            btnReceive.Click += delegate (object s, EventArgs e) { ReceiveSelectedPurchaseOrder(); };

            toolbar.Controls.Add(lblHint);
            toolbar.Controls.Add(btnReceive);

            _dgvOrders = CreateStyledGrid();
            _dgvOrders.Columns.Add("PoNum", "PO Number");
            _dgvOrders.Columns.Add("Supplier", "Supplier (Discreet Vendor)");
            _dgvOrders.Columns.Add("Sku", "Target SKU");
            _dgvOrders.Columns.Add("Summary", "Item Summary");
            _dgvOrders.Columns.Add("Qty", "Order Qty");
            _dgvOrders.Columns.Add("Cost", "Total Cost");
            _dgvOrders.Columns.Add("Expected", "Expected Date");
            _dgvOrders.Columns.Add("Status", "Status");

            for (int i = 0; i < _purchaseOrders.Count; i++)
            {
                PurchaseOrder po = _purchaseOrders[i];
                _dgvOrders.Rows.Add(
                    po.PoNumber,
                    po.Supplier,
                    po.TargetSku,
                    _discreetMode ? string.Format("Wellness Restock ({0})", po.TargetSku) : po.ProductSummary,
                    po.Quantity,
                    string.Format("${0:N2}", po.TotalCost),
                    po.ExpectedDate.ToString("yyyy-MM-dd"),
                    po.Status
                );
            }

            ApplyStatusBadgeFormatting(_dgvOrders, 7);
            card.Controls.Add(_dgvOrders);
            card.Controls.Add(toolbar);
            _contentHost.Controls.Add(card);
        }

        private void ReceiveSelectedPurchaseOrder()
        {
            if (_dgvOrders.SelectedRows.Count == 0)
            {
                MessageBox.Show("Please select a Purchase Order row first.", "Select Order", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string poNum = _dgvOrders.SelectedRows[0].Cells[0].Value.ToString();
            PurchaseOrder po = null;
            for (int i = 0; i < _purchaseOrders.Count; i++)
            {
                if (_purchaseOrders[i].PoNumber == poNum) { po = _purchaseOrders[i]; break; }
            }

            if (po == null || po.Status == "Received")
            {
                MessageBox.Show("This Purchase Order has already been received.", "Already Completed", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ProductItem product = null;
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].Sku == po.TargetSku) { product = _products[i]; break; }
            }

            if (product != null)
            {
                product.Stock += po.Quantity;
                po.Status = "Received";

                AuditLogEntry entry = new AuditLogEntry();
                entry.Timestamp = DateTime.Now;
                entry.Sku = product.Sku;
                entry.ProductName = product.Name;
                entry.ActionType = "PO_RECEIPT";
                entry.QuantityDelta = po.Quantity;
                entry.BalanceAfter = product.Stock;
                entry.Notes = string.Format("Received {0} from {1}", po.PoNumber, po.Supplier);
                _auditLogs.Insert(0, entry);

                MessageBox.Show(string.Format("Received +{0} units of {1}.", po.Quantity, product.Sku), "Stock Updated", MessageBoxButtons.OK, MessageBoxIcon.Information);
                RefreshActiveView();
            }
        }

        private void RenderAuditLogView()
        {
            Panel card = CreateCardContainer();
            Panel toolbar = new Panel();
            toolbar.Dock = DockStyle.Top;
            toolbar.Height = 46;
            toolbar.Padding = new Padding(0, 0, 0, 10);

            Label lblInfo = new Label();
            lblInfo.Text = "Immutable stock movement trail (FEFO deductions, manual cycle counts, and PO receipts).";
            lblInfo.ForeColor = BrandTheme.TextMuted;
            lblInfo.Dock = DockStyle.Left;
            lblInfo.AutoSize = true;

            Button btnExport = CreatePrimaryButton("Export Audit Log (.CSV)", 195);
            btnExport.Dock = DockStyle.Right;
            btnExport.Click += delegate (object s, EventArgs e) { ExportAuditCsv(); };

            toolbar.Controls.Add(lblInfo);
            toolbar.Controls.Add(btnExport);

            _dgvAudit = CreateStyledGrid();
            _dgvAudit.Columns.Add("Time", "Timestamp");
            _dgvAudit.Columns.Add("Sku", "SKU");
            _dgvAudit.Columns.Add("Name", "Product");
            _dgvAudit.Columns.Add("Type", "Movement Type");
            _dgvAudit.Columns.Add("Delta", "Qty Change");
            _dgvAudit.Columns.Add("Balance", "New Balance");
            _dgvAudit.Columns.Add("Notes", "Audit Notes");

            for (int i = 0; i < _auditLogs.Count; i++)
            {
                AuditLogEntry log = _auditLogs[i];
                string deltaStr = log.QuantityDelta > 0 ? "+" + log.QuantityDelta : log.QuantityDelta.ToString();
                _dgvAudit.Rows.Add(
                    log.Timestamp.ToString("yyyy-MM-dd HH:mm"),
                    log.Sku,
                    _discreetMode ? string.Format("Masked Item ({0})", log.Sku) : log.ProductName,
                    log.ActionType,
                    deltaStr,
                    log.BalanceAfter,
                    log.Notes
                );
            }

            card.Controls.Add(_dgvAudit);
            card.Controls.Add(toolbar);
            _contentHost.Controls.Add(card);
        }

        private void ExportAuditCsv()
        {
            using (SaveFileDialog sfd = new SaveFileDialog())
            {
                sfd.Filter = "CSV Files (*.csv)|*.csv";
                sfd.FileName = string.Format("AuraWellness_AuditLog_{0:yyyyMMdd}.csv", DateTime.Now);

                if (sfd.ShowDialog() == DialogResult.OK)
                {
                    StringBuilder sb = new StringBuilder();
                    sb.AppendLine("Timestamp,SKU,Product,MovementType,QuantityChange,BalanceAfter,Notes");
                    for (int i = 0; i < _auditLogs.Count; i++)
                    {
                        AuditLogEntry log = _auditLogs[i];
                        string safeName = _discreetMode ? "Item-" + log.Sku : log.ProductName.Replace(",", " ");
                        sb.AppendLine(string.Format("{0:O},{1},{2},{3},{4},{5},\"{6}\"", log.Timestamp, log.Sku, safeName, log.ActionType, log.QuantityDelta, log.BalanceAfter, log.Notes));
                    }
                    File.WriteAllText(sfd.FileName, sb.ToString());
                    MessageBox.Show("Audit log exported cleanly.", "Export Complete", MessageBoxButtons.OK, MessageBoxIcon.Information);
                }
            }
        }

        private void AdjustSelectedStock()
        {
            if (_dgvCatalog.SelectedRows.Count == 0)
            {
                MessageBox.Show("Select a product row first.", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string sku = _dgvCatalog.SelectedRows[0].Cells[0].Value.ToString();
            ProductItem product = null;
            for (int i = 0; i < _products.Count; i++)
            {
                if (_products[i].Sku == sku) { product = _products[i]; break; }
            }
            if (product == null) return;

            using (Form dlg = new Form())
            {
                dlg.Text = "Adjust Stock - " + product.Sku;
                dlg.Size = new Size(380, 270);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.BackColor = Color.White;

                Label lbl = new Label();
                lbl.Text = string.Format("Current Balance: {0} units\nEnter adjustment (+/- quantity):", product.Stock);
                lbl.Location = new Point(20, 18);
                lbl.AutoSize = true;

                NumericUpDown numDelta = new NumericUpDown();
                numDelta.Minimum = -500;
                numDelta.Maximum = 500;
                numDelta.Value = -1;
                numDelta.Location = new Point(20, 62);
                numDelta.Width = 320;

                Label lblReason = new Label();
                lblReason.Text = "Reason / Notes:";
                lblReason.Location = new Point(20, 95);
                lblReason.AutoSize = true;

                TextBox txtReason = new TextBox();
                txtReason.Location = new Point(20, 116);
                txtReason.Width = 320;

                Button btnSave = CreatePrimaryButton("Apply Adjustment", 320);
                btnSave.Location = new Point(20, 160);
                btnSave.Click += delegate (object s, EventArgs e)
                {
                    int delta = (int)numDelta.Value;
                    if (product.Stock + delta < 0)
                    {
                        MessageBox.Show("Stock cannot drop below zero.", "Invalid Quantity", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    product.Stock += delta;

                    AuditLogEntry entry = new AuditLogEntry();
                    entry.Timestamp = DateTime.Now;
                    entry.Sku = product.Sku;
                    entry.ProductName = product.Name;
                    entry.ActionType = delta < 0 ? "SALE_OR_DEDUCTION" : "CYCLE_ADJUSTMENT";
                    entry.QuantityDelta = delta;
                    entry.BalanceAfter = product.Stock;
                    entry.Notes = string.IsNullOrWhiteSpace(txtReason.Text) ? "Manual counter adjustment" : txtReason.Text;
                    _auditLogs.Insert(0, entry);
                    dlg.DialogResult = DialogResult.OK;
                };

                dlg.Controls.AddRange(new Control[] { lbl, numDelta, lblReason, txtReason, btnSave });
                if (dlg.ShowDialog() == DialogResult.OK) PopulateCatalogGrid();
            }
        }

        private void ShowAddProductDialog()
        {
            using (Form dlg = new Form())
            {
                dlg.Text = "Add New Wellness SKU";
                dlg.Size = new Size(420, 440);
                dlg.StartPosition = FormStartPosition.CenterParent;
                dlg.FormBorderStyle = FormBorderStyle.FixedDialog;
                dlg.MaximizeBox = false;
                dlg.MinimizeBox = false;
                dlg.BackColor = Color.White;

                Label lbl1 = new Label() { Text = "SKU Code:", Location = new Point(20, 15), AutoSize = true };
                TextBox txtSku = new TextBox() { Location = new Point(20, 35), Width = 360 };
                Label lbl2 = new Label() { Text = "Full Product Name:", Location = new Point(20, 65), AutoSize = true };
                TextBox txtName = new TextBox() { Location = new Point(20, 85), Width = 360 };
                Label lbl3 = new Label() { Text = "Discreet Alias:", Location = new Point(20, 115), AutoSize = true };
                TextBox txtDiscreet = new TextBox() { Location = new Point(20, 135), Width = 360 };

                ComboBox cmbCat = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, 172), Width = 360 };
                cmbCat.Items.AddRange(new object[] { "Personal Massagers", "Intimate Wellness", "Lubricants & Care", "Couples & Accessories" });
                cmbCat.SelectedIndex = 0;

                ComboBox cmbMat = new ComboBox() { DropDownStyle = ComboBoxStyle.DropDownList, Location = new Point(20, 210), Width = 360 };
                cmbMat.Items.AddRange(new object[] { "Medical-Grade Silicone", "Borosilicate Glass", "Body-Safe ABS", "Water-Based Formula" });
                cmbMat.SelectedIndex = 0;

                Label lblQty = new Label() { Text = "Initial Stock:", Location = new Point(20, 245), AutoSize = true };
                NumericUpDown numInitStock = new NumericUpDown() { Minimum = 0, Maximum = 5000, Value = 25, Location = new Point(20, 265), Width = 170 };
                Label lblPrice = new Label() { Text = "Retail Price ($):", Location = new Point(210, 245), AutoSize = true };
                NumericUpDown numPrice = new NumericUpDown() { Minimum = 1, Maximum = 1000, DecimalPlaces = 2, Value = 65.00m, Location = new Point(210, 265), Width = 170 };

                Button btnCreate = CreatePrimaryButton("Save Product to Catalog", 360);
                btnCreate.Location = new Point(20, 315);
                btnCreate.Click += delegate (object s, EventArgs e)
                {
                    if (string.IsNullOrWhiteSpace(txtSku.Text) || string.IsNullOrWhiteSpace(txtName.Text))
                    {
                        MessageBox.Show("SKU and Product Name are required.", "Missing Fields", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }

                    ProductItem newItem = new ProductItem();
                    newItem.Sku = txtSku.Text.Trim().ToUpper();
                    newItem.Name = txtName.Text.Trim();
                    newItem.DiscreetName = string.IsNullOrWhiteSpace(txtDiscreet.Text) ? string.Format("Wellness Item ({0})", txtSku.Text.Trim()) : txtDiscreet.Text.Trim();
                    newItem.Category = cmbCat.SelectedItem.ToString();
                    newItem.Material = cmbMat.SelectedItem.ToString();
                    newItem.WaterproofRating = "IPX7";
                    newItem.LotNumber = string.Format("LOT-{0:yyyyMM}", DateTime.Now);
                    newItem.ExpiryDate = (cmbMat.SelectedItem.ToString() == "Water-Based Formula") ? (DateTime?)DateTime.Today.AddMonths(18) : null;
                    newItem.Stock = (int)numInitStock.Value;
                    newItem.ReorderPoint = 10;
                    newItem.UnitCost = Math.Round(numPrice.Value * 0.42m, 2);
                    newItem.RetailPrice = numPrice.Value;

                    _products.Add(newItem);

                    AuditLogEntry entry = new AuditLogEntry();
                    entry.Timestamp = DateTime.Now;
                    entry.Sku = newItem.Sku;
                    entry.ProductName = newItem.Name;
                    entry.ActionType = "INITIAL_STOCK";
                    entry.QuantityDelta = newItem.Stock;
                    entry.BalanceAfter = newItem.Stock;
                    entry.Notes = "Created new catalog SKU";
                    _auditLogs.Insert(0, entry);
                    dlg.DialogResult = DialogResult.OK;
                };

                dlg.Controls.AddRange(new Control[] { lbl1, txtSku, lbl2, txtName, lbl3, txtDiscreet, cmbCat, cmbMat, lblQty, numInitStock, lblPrice, numPrice, btnCreate });
                if (dlg.ShowDialog() == DialogResult.OK) PopulateCatalogGrid();
            }
        }

        private Panel CreateCardContainer()
        {
            Panel panel = new Panel();
            panel.Dock = DockStyle.Fill;
            panel.BackColor = BrandTheme.CardWhite;
            panel.Padding = new Padding(20);
            panel.Paint += delegate (object s, PaintEventArgs e)
            {
                using (Pen pen = new Pen(BrandTheme.BorderSubtle, 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, panel.Width - 1, panel.Height - 1);
                }
            };
            return panel;
        }

        private Panel CreateKpiCard(string title, string bigValue, string subtitle, Color accentColor)
        {
            Panel card = new Panel();
            card.Dock = DockStyle.Fill;
            card.BackColor = BrandTheme.CardWhite;
            card.Margin = new Padding(0, 0, 12, 0);
            card.Padding = new Padding(16);
            card.Paint += delegate (object s, PaintEventArgs e)
            {
                using (Pen pen = new Pen(BrandTheme.BorderSubtle, 1))
                {
                    e.Graphics.DrawRectangle(pen, 0, 0, card.Width - 1, card.Height - 1);
                }
            };

            Label lblTitle = new Label() { Text = title, Font = new Font("Segoe UI", 7.5f, FontStyle.Bold), ForeColor = BrandTheme.TextMuted, Dock = DockStyle.Top, Height = 20 };
            Label lblVal = new Label() { Text = bigValue, Font = new Font("Segoe UI", 17f, FontStyle.Bold), ForeColor = accentColor, Dock = DockStyle.Top, Height = 36 };
            Label lblSub = new Label() { Text = subtitle, Font = new Font("Segoe UI", 8.5f), ForeColor = BrandTheme.TextMuted, Dock = DockStyle.Top, Height = 20 };

            card.Controls.Add(lblSub);
            card.Controls.Add(lblVal);
            card.Controls.Add(lblTitle);
            return card;
        }

        private Button CreatePrimaryButton(string text, int width)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Width = width;
            btn.Height = 34;
            btn.BackColor = BrandTheme.PrimaryTeal;
            btn.ForeColor = Color.White;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = BrandTheme.BoldBodyFont;
            btn.Cursor = Cursors.Hand;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = BrandTheme.PrimaryHover;
            return btn;
        }

        private Button CreateSecondaryButton(string text, int width)
        {
            Button btn = new Button();
            btn.Text = text;
            btn.Width = width;
            btn.Height = 34;
            btn.BackColor = BrandTheme.CanvasStone;
            btn.ForeColor = BrandTheme.TextDark;
            btn.FlatStyle = FlatStyle.Flat;
            btn.Font = BrandTheme.BoldBodyFont;
            btn.Cursor = Cursors.Hand;
            btn.Margin = new Padding(0, 0, 8, 0);
            btn.FlatAppearance.BorderColor = BrandTheme.BorderSubtle;
            return btn;
        }

        private DataGridView CreateStyledGrid()
        {
            DataGridView dgv = new DataGridView();
            dgv.Dock = DockStyle.Fill;
            dgv.BackgroundColor = BrandTheme.CardWhite;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.GridColor = BrandTheme.BorderSubtle;
            dgv.EnableHeadersVisualStyles = false;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dgv.ColumnHeadersHeight = 40;
            dgv.RowTemplate.Height = 38;
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.ReadOnly = true;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.MultiSelect = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.RowHeadersVisible = false;

            dgv.ColumnHeadersDefaultCellStyle.BackColor = BrandTheme.CanvasStone;
            dgv.ColumnHeadersDefaultCellStyle.ForeColor = BrandTheme.TextMuted;
            dgv.ColumnHeadersDefaultCellStyle.Font = new Font("Segoe UI", 8.5f, FontStyle.Bold);

            dgv.DefaultCellStyle.BackColor = BrandTheme.CardWhite;
            dgv.DefaultCellStyle.ForeColor = BrandTheme.TextDark;
            dgv.DefaultCellStyle.SelectionBackColor = BrandTheme.SoftTealBg;
            dgv.DefaultCellStyle.SelectionForeColor = BrandTheme.SoftTealText;
            dgv.DefaultCellStyle.Font = BrandTheme.BodyFont;

            return dgv;
        }

        private void ApplyStatusBadgeFormatting(DataGridView dgv, int statusColumnIndex)
        {
            dgv.CellFormatting += delegate (object s, DataGridViewCellFormattingEventArgs e)
            {
                if (e.ColumnIndex == 0 && e.CellStyle != null)
                {
                    e.CellStyle.Font = BrandTheme.MonoFont;
                }

                if (e.ColumnIndex == statusColumnIndex && e.Value != null)
                {
                    string status = e.Value.ToString();
                    if (status == "In Stock" || status == "Received")
                    {
                        e.CellStyle.BackColor = BrandTheme.SuccessBg;
                        e.CellStyle.ForeColor = BrandTheme.SuccessText;
                        e.CellStyle.Font = BrandTheme.BoldBodyFont;
                    }
                    else if (status == "Low Stock" || status == "Out of Stock")
                    {
                        e.CellStyle.BackColor = BrandTheme.DangerBg;
                        e.CellStyle.ForeColor = BrandTheme.DangerText;
                        e.CellStyle.Font = BrandTheme.BoldBodyFont;
                    }
                    else if (status == "Expiring Soon" || status == "Ordered")
                    {
                        e.CellStyle.BackColor = BrandTheme.WarningBg;
                        e.CellStyle.ForeColor = BrandTheme.WarningText;
                        e.CellStyle.Font = BrandTheme.BoldBodyFont;
                    }
                }
            };
        }

        private void SeedDemoData()
        {
            _products = new List<ProductItem>
            {
                new ProductItem { Sku = "MAS-SIL-001", Name = "Velvet Pulse Dual Wand", DiscreetName = "Personal Care Device #101", Category = "Personal Massagers", Material = "Medical-Grade Silicone", WaterproofRating = "IPX7", LotNumber = "LOT-26A1", ExpiryDate = null, Stock = 34, ReorderPoint = 12, UnitCost = 38.00m, RetailPrice = 98.00m },
                new ProductItem { Sku = "WEL-GLS-002", Name = "Lumiere Sculpted Borosilicate Wand", DiscreetName = "Glass Wellness Item #204", Category = "Intimate Wellness", Material = "Borosilicate Glass", WaterproofRating = "IPX8", LotNumber = "LOT-26B4", ExpiryDate = null, Stock = 8, ReorderPoint = 10, UnitCost = 24.00m, RetailPrice = 64.00m },
                new ProductItem { Sku = "LUB-WTR-003", Name = "SilkGlide Aloe Organic Serum (100ml)", DiscreetName = "Botanical Care Gel #301", Category = "Lubricants & Care", Material = "Water-Based Formula", WaterproofRating = "N/A", LotNumber = "LOT-25X9", ExpiryDate = DateTime.Today.AddDays(28), Stock = 42, ReorderPoint = 15, UnitCost = 7.50m, RetailPrice = 22.00m },
                new ProductItem { Sku = "CPL-SIL-004", Name = "AuraSync App-Controlled Couple Ring", DiscreetName = "Rechargeable Wellness Band #402", Category = "Couples & Accessories", Material = "Medical-Grade Silicone", WaterproofRating = "IPX7", LotNumber = "LOT-26C2", ExpiryDate = null, Stock = 0, ReorderPoint = 8, UnitCost = 44.00m, RetailPrice = 115.00m },
                new ProductItem { Sku = "LUB-CLN-005", Name = "PureMist Antimicrobial Toy Cleanser", DiscreetName = "Gentle Surface Mist #309", Category = "Lubricants & Care", Material = "Alcohol-Free Formula", WaterproofRating = "N/A", LotNumber = "LOT-26D1", ExpiryDate = DateTime.Today.AddMonths(14), Stock = 65, ReorderPoint = 20, UnitCost = 5.00m, RetailPrice = 16.00m }
            };

            _purchaseOrders = new List<PurchaseOrder>
            {
                new PurchaseOrder { PoNumber = "PO-2026-081", Supplier = "Nordic Botanicals Logistics", TargetSku = "CPL-SIL-004", ProductSummary = "AuraSync Couple Ring (Restock)", Quantity = 20, TotalCost = 880.00m, Status = "Ordered", ExpectedDate = DateTime.Today.AddDays(2) },
                new PurchaseOrder { PoNumber = "PO-2026-082", Supplier = "Vespera Glassworks Ltd.", TargetSku = "WEL-GLS-002", ProductSummary = "Lumiere Borosilicate Wand", Quantity = 15, TotalCost = 360.00m, Status = "Ordered", ExpectedDate = DateTime.Today.AddDays(4) },
                new PurchaseOrder { PoNumber = "PO-2026-079", Supplier = "PureFormulations Co.", TargetSku = "LUB-CLN-005", ProductSummary = "PureMist Toy Cleanser (120ml)", Quantity = 50, TotalCost = 250.00m, Status = "Received", ExpectedDate = DateTime.Today.AddDays(-3) }
            };

            _auditLogs = new List<AuditLogEntry>
            {
                new AuditLogEntry { Timestamp = DateTime.Now.AddHours(-2), Sku = "MAS-SIL-001", ProductName = "Velvet Pulse Dual Wand", ActionType = "POS_SALE", QuantityDelta = -2, BalanceAfter = 34, Notes = "In-store boutique checkout #4419" },
                new AuditLogEntry { Timestamp = DateTime.Now.AddDays(-1), Sku = "LUB-WTR-003", ProductName = "SilkGlide Aloe Organic Serum", ActionType = "FEFO_PICK", QuantityDelta = -3, BalanceAfter = 42, Notes = "Picked earliest expiry LOT-25X9" },
                new AuditLogEntry { Timestamp = DateTime.Now.AddDays(-3), Sku = "LUB-CLN-005", ProductName = "PureMist Antimicrobial Toy Cleanser", ActionType = "PO_RECEIPT", QuantityDelta = 50, BalanceAfter = 65, Notes = "Received PO-2026-079" }
            };
        }
    }
}