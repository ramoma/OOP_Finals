#nullable disable
using System.Drawing;
using System.Windows.Forms;

namespace OOPInventoryCatalog
{
    partial class Form1
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            pnlSidebar = new Panel();
            badgeLogo = new Badge();
            lblBrand = new Label();
            navDashboard = new NavButton();
            navCatalog = new NavButton();
            navSuppliers = new NavButton();
            navAudit = new NavButton();
            pnlMain = new Panel();
            pnlContent = new Panel();
            tlpContent = new TableLayoutPanel();
            pnlHeader = new Panel();
            lblTitle = new Label();
            lblSubtitle = new Label();
            btnImport = new OutlineButton();
            btnExport = new OutlineButton();
            tlpKpis = new TableLayoutPanel();
            kpiCapacity = new KpiCard();
            kpiValue = new KpiCard();
            kpiLow = new KpiCard();
            kpiOut = new KpiCard();
            cardTable = new RoundedPanel();
            dgv = new DataGridView();
            colProduct = new DataGridViewTextBoxColumn();
            colCategory = new DataGridViewTextBoxColumn();
            colCost = new DataGridViewTextBoxColumn();
            colPrice = new DataGridViewTextBoxColumn();
            colMargin = new DataGridViewTextBoxColumn();
            colStock = new DataGridViewTextBoxColumn();
            colWarehouses = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            colAction = new DataGridViewTextBoxColumn();
            pnlPager = new Panel();
            lblPageInfo = new Label();
            btnPrev = new OutlineButton();
            btnNext = new OutlineButton();
            pnlToolbar = new Panel();
            flpTabs = new FlowLayoutPanel();
            btnTabAll = new TabButton();
            btnTabHealthy = new TabButton();
            btnTabLow = new TabButton();
            btnTabOut = new TabButton();
            btnTabOverstock = new TabButton();
            cboCategory = new ComboBox();
            cboWarehouse = new ComboBox();
            pnlTopBar = new Panel();
            pnlSearch = new RoundedPanel();
            txtSearch = new TextBox();
            btnAddProduct = new RoundButton();
            btnBell = new OutlineButton();
            pnlProfile = new Panel();
            badgeAvatar = new Badge();
            lblUserName = new Label();
            lblUserRole = new Label();
            pnlSidebar.SuspendLayout();
            pnlMain.SuspendLayout();
            pnlContent.SuspendLayout();
            tlpContent.SuspendLayout();
            pnlHeader.SuspendLayout();
            tlpKpis.SuspendLayout();
            cardTable.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)dgv).BeginInit();
            pnlPager.SuspendLayout();
            pnlToolbar.SuspendLayout();
            flpTabs.SuspendLayout();
            pnlTopBar.SuspendLayout();
            pnlSearch.SuspendLayout();
            pnlProfile.SuspendLayout();
            SuspendLayout();
            // 
            // pnlSidebar
            // 
            pnlSidebar.BackColor = Color.White;
            pnlSidebar.Controls.Add(badgeLogo);
            pnlSidebar.Controls.Add(lblBrand);
            pnlSidebar.Controls.Add(navDashboard);
            pnlSidebar.Controls.Add(navCatalog);
            pnlSidebar.Controls.Add(navSuppliers);
            pnlSidebar.Controls.Add(navAudit);
            pnlSidebar.Dock = DockStyle.Left;
            pnlSidebar.Location = new Point(0, 0);
            pnlSidebar.Name = "pnlSidebar";
            pnlSidebar.Size = new Size(216, 834);
            pnlSidebar.TabIndex = 1;
            pnlSidebar.Paint += pnlSidebar_Paint;
            // 
            // badgeLogo
            // 
            badgeLogo.BackColor = Color.Transparent;
            badgeLogo.Fill = Color.FromArgb(11, 150, 160);
            badgeLogo.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            badgeLogo.Location = new Point(20, 18);
            badgeLogo.Name = "badgeLogo";
            badgeLogo.Radius = 8;
            badgeLogo.Size = new Size(28, 28);
            badgeLogo.TabIndex = 0;
            // 
            // lblBrand
            // 
            lblBrand.AutoSize = true;
            lblBrand.BackColor = Color.Transparent;
            lblBrand.Font = new Font("Segoe UI", 17.3333F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblBrand.Location = new Point(56, 20);
            lblBrand.Name = "lblBrand";
            lblBrand.Size = new Size(104, 25);
            lblBrand.TabIndex = 1;
            lblBrand.Text = "Stockroom";
            // 
            // navDashboard
            // 
            navDashboard.BackColor = Color.Transparent;
            navDashboard.BorderColor = Color.Transparent;
            navDashboard.FillColor = Color.Transparent;
            navDashboard.FlatStyle = FlatStyle.Flat;
            navDashboard.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navDashboard.HoverFill = Color.FromArgb(9, 125, 133);
            navDashboard.HoverText = Color.White;
            navDashboard.IconName = "grid";
            navDashboard.Location = new Point(12, 80);
            navDashboard.Name = "navDashboard";
            navDashboard.Radius = 22;
            navDashboard.Selected = false;
            navDashboard.SelectedFill = Color.FromArgb(11, 150, 160);
            navDashboard.SelectedText = Color.FromArgb(30, 30, 30);
            navDashboard.Size = new Size(192, 44);
            navDashboard.TabIndex = 2;
            navDashboard.Text = "Dashboard";
            navDashboard.TextColor = Color.FromArgb(30, 30, 30);
            navDashboard.UseVisualStyleBackColor = false;
            // 
            // navCatalog
            // 
            navCatalog.BackColor = Color.Transparent;
            navCatalog.BorderColor = Color.Transparent;
            navCatalog.FillColor = Color.Transparent;
            navCatalog.FlatStyle = FlatStyle.Flat;
            navCatalog.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navCatalog.HoverFill = Color.FromArgb(9, 125, 133);
            navCatalog.HoverText = Color.White;
            navCatalog.IconName = "box";
            navCatalog.Location = new Point(12, 128);
            navCatalog.Name = "navCatalog";
            navCatalog.Radius = 22;
            navCatalog.Selected = true;
            navCatalog.SelectedFill = Color.FromArgb(11, 150, 160);
            navCatalog.SelectedText = Color.FromArgb(30, 30, 30);
            navCatalog.Size = new Size(192, 44);
            navCatalog.TabIndex = 3;
            navCatalog.Text = "Inventory Catalog";
            navCatalog.TextColor = Color.FromArgb(30, 30, 30);
            navCatalog.UseVisualStyleBackColor = false;
            // 
            // navSuppliers
            // 
            navSuppliers.BackColor = Color.Transparent;
            navSuppliers.BorderColor = Color.Transparent;
            navSuppliers.FillColor = Color.Transparent;
            navSuppliers.FlatStyle = FlatStyle.Flat;
            navSuppliers.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navSuppliers.HoverFill = Color.FromArgb(9, 125, 133);
            navSuppliers.HoverText = Color.White;
            navSuppliers.IconName = "truck";
            navSuppliers.Location = new Point(12, 176);
            navSuppliers.Name = "navSuppliers";
            navSuppliers.Radius = 22;
            navSuppliers.Selected = false;
            navSuppliers.SelectedFill = Color.FromArgb(11, 150, 160);
            navSuppliers.SelectedText = Color.FromArgb(30, 30, 30);
            navSuppliers.Size = new Size(192, 44);
            navSuppliers.TabIndex = 4;
            navSuppliers.Text = "Suppliers & POs";
            navSuppliers.TextColor = Color.FromArgb(30, 30, 30);
            navSuppliers.UseVisualStyleBackColor = false;
            // 
            // navAudit
            // 
            navAudit.BackColor = Color.Transparent;
            navAudit.BorderColor = Color.Transparent;
            navAudit.FillColor = Color.Transparent;
            navAudit.FlatStyle = FlatStyle.Flat;
            navAudit.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            navAudit.HoverFill = Color.FromArgb(9, 125, 133);
            navAudit.HoverText = Color.White;
            navAudit.IconName = "log";
            navAudit.Location = new Point(12, 224);
            navAudit.Name = "navAudit";
            navAudit.Radius = 22;
            navAudit.Selected = false;
            navAudit.SelectedFill = Color.FromArgb(11, 150, 160);
            navAudit.SelectedText = Color.FromArgb(30, 30, 30);
            navAudit.Size = new Size(192, 44);
            navAudit.TabIndex = 5;
            navAudit.Text = "Audit Movement Log";
            navAudit.TextColor = Color.FromArgb(30, 30, 30);
            navAudit.UseVisualStyleBackColor = false;
            // 
            // pnlMain
            // 
            pnlMain.BackColor = Color.FromArgb(245, 245, 244);
            pnlMain.Controls.Add(pnlContent);
            pnlMain.Controls.Add(pnlTopBar);
            pnlMain.Dock = DockStyle.Fill;
            pnlMain.Location = new Point(216, 0);
            pnlMain.Name = "pnlMain";
            pnlMain.Size = new Size(978, 834);
            pnlMain.TabIndex = 0;
            // 
            // pnlContent
            // 
            pnlContent.BackColor = Color.FromArgb(245, 245, 244);
            pnlContent.Controls.Add(tlpContent);
            pnlContent.Dock = DockStyle.Fill;
            pnlContent.Location = new Point(0, 64);
            pnlContent.Name = "pnlContent";
            pnlContent.Padding = new Padding(24, 16, 24, 20);
            pnlContent.Size = new Size(978, 770);
            pnlContent.TabIndex = 1;
            // 
            // tlpContent
            // 
            tlpContent.BackColor = Color.FromArgb(245, 245, 244);
            tlpContent.ColumnCount = 1;
            tlpContent.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpContent.Controls.Add(pnlHeader, 0, 0);
            tlpContent.Controls.Add(tlpKpis, 0, 1);
            tlpContent.Controls.Add(cardTable, 0, 2);
            tlpContent.Dock = DockStyle.Fill;
            tlpContent.Location = new Point(24, 16);
            tlpContent.Name = "tlpContent";
            tlpContent.RowCount = 3;
            tlpContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 60F));
            tlpContent.RowStyles.Add(new RowStyle(SizeType.Absolute, 116F));
            tlpContent.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpContent.Size = new Size(930, 734);
            tlpContent.TabIndex = 0;
            // 
            // pnlHeader
            // 
            pnlHeader.BackColor = Color.FromArgb(245, 245, 244);
            pnlHeader.Controls.Add(lblTitle);
            pnlHeader.Controls.Add(lblSubtitle);
            pnlHeader.Controls.Add(btnImport);
            pnlHeader.Controls.Add(btnExport);
            pnlHeader.Dock = DockStyle.Fill;
            pnlHeader.Location = new Point(0, 0);
            pnlHeader.Margin = new Padding(0, 0, 0, 8);
            pnlHeader.Name = "pnlHeader";
            pnlHeader.Size = new Size(930, 52);
            pnlHeader.TabIndex = 0;
            // 
            // lblTitle
            // 
            lblTitle.AutoSize = true;
            lblTitle.Font = new Font("Segoe UI", 24F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblTitle.Location = new Point(0, 0);
            lblTitle.Name = "lblTitle";
            lblTitle.Size = new Size(226, 35);
            lblTitle.TabIndex = 0;
            lblTitle.Text = "Inventory Catalog";
            // 
            // lblSubtitle
            // 
            lblSubtitle.AutoSize = true;
            lblSubtitle.ForeColor = Color.FromArgb(107, 107, 107);
            lblSubtitle.Location = new Point(2, 34);
            lblSubtitle.Name = "lblSubtitle";
            lblSubtitle.Size = new Size(388, 17);
            lblSubtitle.TabIndex = 1;
            lblSubtitle.Text = "Manage SKUs, unit costs, pricing, and live warehouse stock levels";
            // 
            // btnImport
            // 
            btnImport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnImport.BackColor = Color.Transparent;
            btnImport.BorderColor = Color.FromArgb(217, 217, 217);
            btnImport.FillColor = Color.White;
            btnImport.FlatStyle = FlatStyle.Flat;
            btnImport.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnImport.HoverFill = Color.FromArgb(230, 246, 247);
            btnImport.HoverText = Color.FromArgb(30, 30, 30);
            btnImport.IconName = "";
            btnImport.Location = new Point(720, 12);
            btnImport.Name = "btnImport";
            btnImport.Radius = 12;
            btnImport.Selected = false;
            btnImport.SelectedFill = Color.FromArgb(11, 150, 160);
            btnImport.SelectedText = Color.FromArgb(30, 30, 30);
            btnImport.Size = new Size(110, 40);
            btnImport.TabIndex = 2;
            btnImport.Text = "Import CSV";
            btnImport.TextColor = Color.FromArgb(30, 30, 30);
            btnImport.UseVisualStyleBackColor = false;
            // 
            // btnExport
            // 
            btnExport.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnExport.BackColor = Color.Transparent;
            btnExport.BorderColor = Color.FromArgb(217, 217, 217);
            btnExport.FillColor = Color.White;
            btnExport.FlatStyle = FlatStyle.Flat;
            btnExport.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnExport.HoverFill = Color.FromArgb(230, 246, 247);
            btnExport.HoverText = Color.FromArgb(30, 30, 30);
            btnExport.IconName = "";
            btnExport.Location = new Point(840, 12);
            btnExport.Name = "btnExport";
            btnExport.Radius = 12;
            btnExport.Selected = false;
            btnExport.SelectedFill = Color.FromArgb(11, 150, 160);
            btnExport.SelectedText = Color.FromArgb(30, 30, 30);
            btnExport.Size = new Size(90, 40);
            btnExport.TabIndex = 3;
            btnExport.Text = "Export";
            btnExport.TextColor = Color.FromArgb(30, 30, 30);
            btnExport.UseVisualStyleBackColor = false;
            // 
            // tlpKpis
            // 
            tlpKpis.BackColor = Color.FromArgb(245, 245, 244);
            tlpKpis.ColumnCount = 4;
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpis.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 25F));
            tlpKpis.Controls.Add(kpiCapacity, 0, 0);
            tlpKpis.Controls.Add(kpiValue, 1, 0);
            tlpKpis.Controls.Add(kpiLow, 2, 0);
            tlpKpis.Controls.Add(kpiOut, 3, 0);
            tlpKpis.Dock = DockStyle.Fill;
            tlpKpis.Location = new Point(0, 60);
            tlpKpis.Margin = new Padding(0, 0, 0, 16);
            tlpKpis.Name = "tlpKpis";
            tlpKpis.RowCount = 1;
            tlpKpis.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpKpis.Size = new Size(930, 100);
            tlpKpis.TabIndex = 1;
            // 
            // kpiCapacity
            // 
            kpiCapacity.Border = Color.FromArgb(217, 217, 217);
            kpiCapacity.Dock = DockStyle.Fill;
            kpiCapacity.Fill = Color.White;
            kpiCapacity.IconBg = Color.FromArgb(230, 246, 247);
            kpiCapacity.IconFg = Color.FromArgb(9, 125, 133);
            kpiCapacity.IconName = "layers";
            kpiCapacity.Link = false;
            kpiCapacity.Location = new Point(0, 0);
            kpiCapacity.Margin = new Padding(0, 0, 12, 0);
            kpiCapacity.Name = "kpiCapacity";
            kpiCapacity.Radius = 16;
            kpiCapacity.Size = new Size(220, 100);
            kpiCapacity.Sub = "";
            kpiCapacity.SubColor = Color.FromArgb(107, 107, 107);
            kpiCapacity.TabIndex = 0;
            kpiCapacity.Title = "Catalog capacity";
            kpiCapacity.Value = "12 SKUs";
            // 
            // kpiValue
            // 
            kpiValue.Border = Color.FromArgb(217, 217, 217);
            kpiValue.Dock = DockStyle.Fill;
            kpiValue.Fill = Color.White;
            kpiValue.IconBg = Color.FromArgb(230, 246, 247);
            kpiValue.IconFg = Color.FromArgb(9, 125, 133);
            kpiValue.IconName = "dollar";
            kpiValue.Link = false;
            kpiValue.Location = new Point(232, 0);
            kpiValue.Margin = new Padding(0, 0, 12, 0);
            kpiValue.Name = "kpiValue";
            kpiValue.Radius = 16;
            kpiValue.Size = new Size(220, 100);
            kpiValue.Sub = "Valued at unit cost";
            kpiValue.SubColor = Color.FromArgb(107, 107, 107);
            kpiValue.TabIndex = 1;
            kpiValue.Title = "Total inventory value";
            kpiValue.Value = "$0.00";
            // 
            // kpiLow
            // 
            kpiLow.Border = Color.FromArgb(217, 217, 217);
            kpiLow.Dock = DockStyle.Fill;
            kpiLow.Fill = Color.White;
            kpiLow.IconBg = Color.FromArgb(255, 251, 235);
            kpiLow.IconFg = Color.FromArgb(180, 83, 9);
            kpiLow.IconName = "warn";
            kpiLow.Link = true;
            kpiLow.Location = new Point(464, 0);
            kpiLow.Margin = new Padding(0, 0, 12, 0);
            kpiLow.Name = "kpiLow";
            kpiLow.Radius = 16;
            kpiLow.Size = new Size(220, 100);
            kpiLow.Sub = "Click to filter catalog >";
            kpiLow.SubColor = Color.FromArgb(9, 125, 133);
            kpiLow.TabIndex = 2;
            kpiLow.Title = "Low stock alerts";
            kpiLow.Value = "0 Items";
            // 
            // kpiOut
            // 
            kpiOut.Border = Color.FromArgb(217, 217, 217);
            kpiOut.Dock = DockStyle.Fill;
            kpiOut.Fill = Color.White;
            kpiOut.IconBg = Color.FromArgb(255, 241, 242);
            kpiOut.IconFg = Color.FromArgb(190, 18, 60);
            kpiOut.IconName = "x";
            kpiOut.Link = true;
            kpiOut.Location = new Point(696, 0);
            kpiOut.Margin = new Padding(0);
            kpiOut.Name = "kpiOut";
            kpiOut.Radius = 16;
            kpiOut.Size = new Size(234, 100);
            kpiOut.Sub = "Immediate restock required >";
            kpiOut.SubColor = Color.FromArgb(190, 18, 60);
            kpiOut.TabIndex = 3;
            kpiOut.Title = "Out of stock";
            kpiOut.Value = "0 Items";
            // 
            // cardTable
            // 
            cardTable.Border = Color.FromArgb(217, 217, 217);
            cardTable.Controls.Add(dgv);
            cardTable.Controls.Add(pnlPager);
            cardTable.Controls.Add(pnlToolbar);
            cardTable.Dock = DockStyle.Fill;
            cardTable.Fill = Color.White;
            cardTable.Location = new Point(0, 176);
            cardTable.Margin = new Padding(0);
            cardTable.Name = "cardTable";
            cardTable.Padding = new Padding(1);
            cardTable.Radius = 16;
            cardTable.Size = new Size(930, 558);
            cardTable.TabIndex = 2;
            // 
            // dgv
            // 
            dgv.AllowUserToAddRows = false;
            dgv.AllowUserToDeleteRows = false;
            dgv.AllowUserToResizeRows = false;
            dgv.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            dgv.BackgroundColor = Color.White;
            dgv.BorderStyle = BorderStyle.None;
            dgv.CellBorderStyle = DataGridViewCellBorderStyle.SingleHorizontal;
            dgv.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.None;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(245, 245, 244);
            dataGridViewCellStyle1.Font = new Font("Segoe UI", 11.3333F, FontStyle.Bold, GraphicsUnit.Pixel);
            dataGridViewCellStyle1.ForeColor = Color.FromArgb(107, 107, 107);
            dataGridViewCellStyle1.Padding = new Padding(8, 0, 8, 0);
            dataGridViewCellStyle1.SelectionBackColor = Color.FromArgb(245, 245, 244);
            dataGridViewCellStyle1.SelectionForeColor = Color.FromArgb(107, 107, 107);
            dgv.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            dgv.ColumnHeadersHeight = 36;
            dgv.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            dgv.Columns.AddRange(new DataGridViewColumn[] { colProduct, colCategory, colCost, colPrice, colMargin, colStock, colWarehouses, colStatus, colAction });
            dataGridViewCellStyle5.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.Font = new Font("Segoe UI", 12.6667F, FontStyle.Regular, GraphicsUnit.Pixel);
            dataGridViewCellStyle5.ForeColor = Color.FromArgb(30, 30, 30);
            dataGridViewCellStyle5.Padding = new Padding(8, 0, 8, 0);
            dataGridViewCellStyle5.SelectionBackColor = Color.FromArgb(230, 246, 247);
            dataGridViewCellStyle5.SelectionForeColor = Color.FromArgb(30, 30, 30);
            dataGridViewCellStyle5.WrapMode = DataGridViewTriState.False;
            dgv.DefaultCellStyle = dataGridViewCellStyle5;
            dgv.Dock = DockStyle.Fill;
            dgv.EnableHeadersVisualStyles = false;
            dgv.GridColor = Color.FromArgb(236, 236, 234);
            dgv.Location = new Point(1, 61);
            dgv.MultiSelect = false;
            dgv.Name = "dgv";
            dgv.ReadOnly = true;
            dgv.RowHeadersVisible = false;
            dgv.RowHeadersWidth = 51;
            dgv.RowTemplate.Height = 48;
            dgv.ScrollBars = ScrollBars.None;
            dgv.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            dgv.Size = new Size(928, 444);
            dgv.TabIndex = 0;
            dgv.CellMouseEnter += dgv_CellMouseEnter;
            dgv.CellPainting += dgv_CellPainting;
            // 
            // colProduct
            // 
            colProduct.FillWeight = 26F;
            colProduct.HeaderText = "PRODUCT";
            colProduct.MinimumWidth = 196;
            colProduct.Name = "colProduct";
            colProduct.ReadOnly = true;
            colProduct.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colCategory
            // 
            colCategory.FillWeight = 11F;
            colCategory.HeaderText = "CATEGORY";
            colCategory.MinimumWidth = 84;
            colCategory.Name = "colCategory";
            colCategory.ReadOnly = true;
            colCategory.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colCost
            // 
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleRight;
            colCost.DefaultCellStyle = dataGridViewCellStyle2;
            colCost.FillWeight = 10F;
            colCost.HeaderText = "COST";
            colCost.MinimumWidth = 72;
            colCost.Name = "colCost";
            colCost.ReadOnly = true;
            colCost.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colPrice
            // 
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleRight;
            colPrice.DefaultCellStyle = dataGridViewCellStyle3;
            colPrice.FillWeight = 10F;
            colPrice.HeaderText = "PRICE";
            colPrice.MinimumWidth = 72;
            colPrice.Name = "colPrice";
            colPrice.ReadOnly = true;
            colPrice.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colMargin
            // 
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleRight;
            colMargin.DefaultCellStyle = dataGridViewCellStyle4;
            colMargin.FillWeight = 7F;
            colMargin.HeaderText = "MARGIN";
            colMargin.MinimumWidth = 62;
            colMargin.Name = "colMargin";
            colMargin.ReadOnly = true;
            colMargin.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colStock
            // 
            colStock.FillWeight = 16F;
            colStock.HeaderText = "STOCK / REORDER";
            colStock.MinimumWidth = 140;
            colStock.Name = "colStock";
            colStock.ReadOnly = true;
            colStock.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colWarehouses
            // 
            colWarehouses.FillWeight = 12F;
            colWarehouses.HeaderText = "WH1 · WH2 · WH3";
            colWarehouses.MinimumWidth = 110;
            colWarehouses.Name = "colWarehouses";
            colWarehouses.ReadOnly = true;
            colWarehouses.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colStatus
            // 
            colStatus.FillWeight = 9F;
            colStatus.HeaderText = "STATUS";
            colStatus.MinimumWidth = 92;
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            colStatus.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // colAction
            // 
            colAction.FillWeight = 9F;
            colAction.HeaderText = "";
            colAction.MinimumWidth = 90;
            colAction.Name = "colAction";
            colAction.ReadOnly = true;
            colAction.SortMode = DataGridViewColumnSortMode.NotSortable;
            // 
            // pnlPager
            // 
            pnlPager.BackColor = Color.White;
            pnlPager.Controls.Add(lblPageInfo);
            pnlPager.Controls.Add(btnPrev);
            pnlPager.Controls.Add(btnNext);
            pnlPager.Dock = DockStyle.Bottom;
            pnlPager.Location = new Point(1, 505);
            pnlPager.Name = "pnlPager";
            pnlPager.Size = new Size(928, 52);
            pnlPager.TabIndex = 1;
            pnlPager.Paint += pnlPager_Paint;
            // 
            // lblPageInfo
            // 
            lblPageInfo.AutoSize = true;
            lblPageInfo.ForeColor = Color.FromArgb(107, 107, 107);
            lblPageInfo.Location = new Point(20, 17);
            lblPageInfo.Name = "lblPageInfo";
            lblPageInfo.Size = new Size(150, 17);
            lblPageInfo.TabIndex = 0;
            lblPageInfo.Text = "Showing 1–8 of 12 SKUs";
            // 
            // btnPrev
            // 
            btnPrev.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnPrev.BackColor = Color.Transparent;
            btnPrev.BorderColor = Color.FromArgb(217, 217, 217);
            btnPrev.FillColor = Color.White;
            btnPrev.FlatStyle = FlatStyle.Flat;
            btnPrev.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnPrev.HoverFill = Color.FromArgb(230, 246, 247);
            btnPrev.HoverText = Color.FromArgb(30, 30, 30);
            btnPrev.IconName = "";
            btnPrev.Location = new Point(728, 6);
            btnPrev.Name = "btnPrev";
            btnPrev.Radius = 12;
            btnPrev.Selected = false;
            btnPrev.SelectedFill = Color.FromArgb(11, 150, 160);
            btnPrev.SelectedText = Color.FromArgb(30, 30, 30);
            btnPrev.Size = new Size(88, 40);
            btnPrev.TabIndex = 1;
            btnPrev.Text = "Previous";
            btnPrev.TextColor = Color.FromArgb(30, 30, 30);
            btnPrev.UseVisualStyleBackColor = false;
            // 
            // btnNext
            // 
            btnNext.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnNext.BackColor = Color.Transparent;
            btnNext.BorderColor = Color.FromArgb(217, 217, 217);
            btnNext.FillColor = Color.White;
            btnNext.FlatStyle = FlatStyle.Flat;
            btnNext.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnNext.HoverFill = Color.FromArgb(230, 246, 247);
            btnNext.HoverText = Color.FromArgb(30, 30, 30);
            btnNext.IconName = "";
            btnNext.Location = new Point(824, 6);
            btnNext.Name = "btnNext";
            btnNext.Radius = 12;
            btnNext.Selected = false;
            btnNext.SelectedFill = Color.FromArgb(11, 150, 160);
            btnNext.SelectedText = Color.FromArgb(30, 30, 30);
            btnNext.Size = new Size(88, 40);
            btnNext.TabIndex = 2;
            btnNext.Text = "Next";
            btnNext.TextColor = Color.FromArgb(30, 30, 30);
            btnNext.UseVisualStyleBackColor = false;
            // 
            // pnlToolbar
            // 
            pnlToolbar.BackColor = Color.White;
            pnlToolbar.Controls.Add(flpTabs);
            pnlToolbar.Controls.Add(cboCategory);
            pnlToolbar.Controls.Add(cboWarehouse);
            pnlToolbar.Dock = DockStyle.Top;
            pnlToolbar.Location = new Point(1, 1);
            pnlToolbar.Name = "pnlToolbar";
            pnlToolbar.Size = new Size(928, 60);
            pnlToolbar.TabIndex = 2;
            pnlToolbar.Paint += pnlToolbar_Paint;
            // 
            // flpTabs
            // 
            flpTabs.AutoSize = true;
            flpTabs.BackColor = Color.White;
            flpTabs.Controls.Add(btnTabAll);
            flpTabs.Controls.Add(btnTabHealthy);
            flpTabs.Controls.Add(btnTabLow);
            flpTabs.Controls.Add(btnTabOut);
            flpTabs.Controls.Add(btnTabOverstock);
            flpTabs.Location = new Point(12, 10);
            flpTabs.Margin = new Padding(0);
            flpTabs.Name = "flpTabs";
            flpTabs.Size = new Size(530, 40);
            flpTabs.TabIndex = 0;
            flpTabs.WrapContents = false;
            // 
            // btnTabAll
            // 
            btnTabAll.BackColor = Color.Transparent;
            btnTabAll.BorderColor = Color.Transparent;
            btnTabAll.FillColor = Color.Transparent;
            btnTabAll.FlatStyle = FlatStyle.Flat;
            btnTabAll.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnTabAll.HoverFill = Color.FromArgb(230, 246, 247);
            btnTabAll.HoverText = Color.FromArgb(30, 30, 30);
            btnTabAll.IconName = "";
            btnTabAll.Location = new Point(0, 0);
            btnTabAll.Margin = new Padding(0, 0, 6, 0);
            btnTabAll.Name = "btnTabAll";
            btnTabAll.Radius = 20;
            btnTabAll.Selected = true;
            btnTabAll.SelectedFill = Color.FromArgb(30, 30, 30);
            btnTabAll.SelectedText = Color.White;
            btnTabAll.Size = new Size(100, 40);
            btnTabAll.TabIndex = 0;
            btnTabAll.Tag = "All";
            btnTabAll.Text = "All";
            btnTabAll.TextColor = Color.FromArgb(30, 30, 30);
            btnTabAll.UseVisualStyleBackColor = false;
            // 
            // btnTabHealthy
            // 
            btnTabHealthy.BackColor = Color.Transparent;
            btnTabHealthy.BorderColor = Color.Transparent;
            btnTabHealthy.FillColor = Color.Transparent;
            btnTabHealthy.FlatStyle = FlatStyle.Flat;
            btnTabHealthy.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnTabHealthy.HoverFill = Color.FromArgb(230, 246, 247);
            btnTabHealthy.HoverText = Color.FromArgb(30, 30, 30);
            btnTabHealthy.IconName = "";
            btnTabHealthy.Location = new Point(106, 0);
            btnTabHealthy.Margin = new Padding(0, 0, 6, 0);
            btnTabHealthy.Name = "btnTabHealthy";
            btnTabHealthy.Radius = 20;
            btnTabHealthy.Selected = false;
            btnTabHealthy.SelectedFill = Color.FromArgb(30, 30, 30);
            btnTabHealthy.SelectedText = Color.White;
            btnTabHealthy.Size = new Size(100, 40);
            btnTabHealthy.TabIndex = 1;
            btnTabHealthy.Tag = "Healthy";
            btnTabHealthy.Text = "Healthy";
            btnTabHealthy.TextColor = Color.FromArgb(30, 30, 30);
            btnTabHealthy.UseVisualStyleBackColor = false;
            btnTabHealthy.Click += btnTabHealthy_Click;
            // 
            // btnTabLow
            // 
            btnTabLow.BackColor = Color.Transparent;
            btnTabLow.BorderColor = Color.Transparent;
            btnTabLow.FillColor = Color.Transparent;
            btnTabLow.FlatStyle = FlatStyle.Flat;
            btnTabLow.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnTabLow.HoverFill = Color.FromArgb(230, 246, 247);
            btnTabLow.HoverText = Color.FromArgb(30, 30, 30);
            btnTabLow.IconName = "";
            btnTabLow.Location = new Point(212, 0);
            btnTabLow.Margin = new Padding(0, 0, 6, 0);
            btnTabLow.Name = "btnTabLow";
            btnTabLow.Radius = 20;
            btnTabLow.Selected = false;
            btnTabLow.SelectedFill = Color.FromArgb(30, 30, 30);
            btnTabLow.SelectedText = Color.White;
            btnTabLow.Size = new Size(100, 40);
            btnTabLow.TabIndex = 2;
            btnTabLow.Tag = "Low";
            btnTabLow.Text = "Low";
            btnTabLow.TextColor = Color.FromArgb(30, 30, 30);
            btnTabLow.UseVisualStyleBackColor = false;
            // 
            // btnTabOut
            // 
            btnTabOut.BackColor = Color.Transparent;
            btnTabOut.BorderColor = Color.Transparent;
            btnTabOut.FillColor = Color.Transparent;
            btnTabOut.FlatStyle = FlatStyle.Flat;
            btnTabOut.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnTabOut.HoverFill = Color.FromArgb(230, 246, 247);
            btnTabOut.HoverText = Color.FromArgb(30, 30, 30);
            btnTabOut.IconName = "";
            btnTabOut.Location = new Point(318, 0);
            btnTabOut.Margin = new Padding(0, 0, 6, 0);
            btnTabOut.Name = "btnTabOut";
            btnTabOut.Radius = 20;
            btnTabOut.Selected = false;
            btnTabOut.SelectedFill = Color.FromArgb(30, 30, 30);
            btnTabOut.SelectedText = Color.White;
            btnTabOut.Size = new Size(100, 40);
            btnTabOut.TabIndex = 3;
            btnTabOut.Tag = "Out";
            btnTabOut.Text = "Out";
            btnTabOut.TextColor = Color.FromArgb(30, 30, 30);
            btnTabOut.UseVisualStyleBackColor = false;
            // 
            // btnTabOverstock
            // 
            btnTabOverstock.BackColor = Color.Transparent;
            btnTabOverstock.BorderColor = Color.Transparent;
            btnTabOverstock.FillColor = Color.Transparent;
            btnTabOverstock.FlatStyle = FlatStyle.Flat;
            btnTabOverstock.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnTabOverstock.HoverFill = Color.FromArgb(230, 246, 247);
            btnTabOverstock.HoverText = Color.FromArgb(30, 30, 30);
            btnTabOverstock.IconName = "";
            btnTabOverstock.Location = new Point(424, 0);
            btnTabOverstock.Margin = new Padding(0, 0, 6, 0);
            btnTabOverstock.Name = "btnTabOverstock";
            btnTabOverstock.Radius = 20;
            btnTabOverstock.Selected = false;
            btnTabOverstock.SelectedFill = Color.FromArgb(30, 30, 30);
            btnTabOverstock.SelectedText = Color.White;
            btnTabOverstock.Size = new Size(100, 40);
            btnTabOverstock.TabIndex = 4;
            btnTabOverstock.Tag = "Overstock";
            btnTabOverstock.Text = "Overstock";
            btnTabOverstock.TextColor = Color.FromArgb(30, 30, 30);
            btnTabOverstock.UseVisualStyleBackColor = false;
            // 
            // cboCategory
            // 
            cboCategory.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboCategory.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategory.FlatStyle = FlatStyle.Flat;
            cboCategory.Location = new Point(602, 17);
            cboCategory.Name = "cboCategory";
            cboCategory.Size = new Size(150, 25);
            cboCategory.TabIndex = 1;
            // 
            // cboWarehouse
            // 
            cboWarehouse.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            cboWarehouse.DropDownStyle = ComboBoxStyle.DropDownList;
            cboWarehouse.FlatStyle = FlatStyle.Flat;
            cboWarehouse.Items.AddRange(new object[] { "All warehouses", "WH1", "WH2", "WH3" });
            cboWarehouse.Location = new Point(762, 17);
            cboWarehouse.Name = "cboWarehouse";
            cboWarehouse.Size = new Size(150, 25);
            cboWarehouse.TabIndex = 2;
            // 
            // pnlTopBar
            // 
            pnlTopBar.BackColor = Color.FromArgb(245, 245, 244);
            pnlTopBar.Controls.Add(pnlSearch);
            pnlTopBar.Controls.Add(btnAddProduct);
            pnlTopBar.Controls.Add(btnBell);
            pnlTopBar.Controls.Add(pnlProfile);
            pnlTopBar.Dock = DockStyle.Top;
            pnlTopBar.Location = new Point(0, 0);
            pnlTopBar.Name = "pnlTopBar";
            pnlTopBar.Size = new Size(978, 64);
            pnlTopBar.TabIndex = 0;
            pnlTopBar.Paint += pnlTopBar_Paint;
            // 
            // pnlSearch
            // 
            pnlSearch.Border = Color.FromArgb(217, 217, 217);
            pnlSearch.Controls.Add(txtSearch);
            pnlSearch.Fill = Color.White;
            pnlSearch.Location = new Point(24, 12);
            pnlSearch.Name = "pnlSearch";
            pnlSearch.Radius = 20;
            pnlSearch.Size = new Size(360, 40);
            pnlSearch.TabIndex = 0;
            // 
            // txtSearch
            // 
            txtSearch.BorderStyle = BorderStyle.None;
            txtSearch.Location = new Point(18, 10);
            txtSearch.Name = "txtSearch";
            txtSearch.PlaceholderText = "Global search by SKU, product name, supplier…";
            txtSearch.Size = new Size(324, 17);
            txtSearch.TabIndex = 0;
            // 
            // btnAddProduct
            // 
            btnAddProduct.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAddProduct.BackColor = Color.Transparent;
            btnAddProduct.BorderColor = Color.Transparent;
            btnAddProduct.FillColor = Color.FromArgb(11, 150, 160);
            btnAddProduct.FlatStyle = FlatStyle.Flat;
            btnAddProduct.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnAddProduct.HoverFill = Color.FromArgb(9, 125, 133);
            btnAddProduct.HoverText = Color.White;
            btnAddProduct.IconName = "";
            btnAddProduct.Location = new Point(564, 12);
            btnAddProduct.Name = "btnAddProduct";
            btnAddProduct.Radius = 12;
            btnAddProduct.Selected = false;
            btnAddProduct.SelectedFill = Color.FromArgb(11, 150, 160);
            btnAddProduct.SelectedText = Color.FromArgb(30, 30, 30);
            btnAddProduct.Size = new Size(140, 40);
            btnAddProduct.TabIndex = 1;
            btnAddProduct.Text = "+ Add Product";
            btnAddProduct.TextColor = Color.FromArgb(30, 30, 30);
            btnAddProduct.UseVisualStyleBackColor = false;
            // 
            // btnBell
            // 
            btnBell.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnBell.BackColor = Color.Transparent;
            btnBell.BorderColor = Color.FromArgb(217, 217, 217);
            btnBell.FillColor = Color.White;
            btnBell.FlatStyle = FlatStyle.Flat;
            btnBell.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            btnBell.HoverFill = Color.FromArgb(230, 246, 247);
            btnBell.HoverText = Color.FromArgb(30, 30, 30);
            btnBell.IconName = "bell";
            btnBell.Location = new Point(716, 12);
            btnBell.Name = "btnBell";
            btnBell.Radius = 12;
            btnBell.Selected = false;
            btnBell.SelectedFill = Color.FromArgb(11, 150, 160);
            btnBell.SelectedText = Color.FromArgb(30, 30, 30);
            btnBell.Size = new Size(40, 40);
            btnBell.TabIndex = 2;
            btnBell.TextColor = Color.FromArgb(30, 30, 30);
            btnBell.UseVisualStyleBackColor = false;
            // 
            // pnlProfile
            // 
            pnlProfile.BackColor = Color.FromArgb(245, 245, 244);
            pnlProfile.Controls.Add(badgeAvatar);
            pnlProfile.Controls.Add(lblUserName);
            pnlProfile.Controls.Add(lblUserRole);
            pnlProfile.Dock = DockStyle.Right;
            pnlProfile.Location = new Point(768, 0);
            pnlProfile.Name = "pnlProfile";
            pnlProfile.Size = new Size(210, 64);
            pnlProfile.TabIndex = 3;
            pnlProfile.Paint += pnlProfile_Paint;
            // 
            // badgeAvatar
            // 
            badgeAvatar.BackColor = Color.Transparent;
            badgeAvatar.Fill = Color.FromArgb(217, 217, 217);
            badgeAvatar.Font = new Font("Segoe UI", 9.5F, FontStyle.Bold);
            badgeAvatar.Location = new Point(14, 12);
            badgeAvatar.Name = "badgeAvatar";
            badgeAvatar.Radius = 10;
            badgeAvatar.Size = new Size(40, 40);
            badgeAvatar.TabIndex = 0;
            badgeAvatar.Text = "AM";
            // 
            // lblUserName
            // 
            lblUserName.AutoSize = true;
            lblUserName.BackColor = Color.Transparent;
            lblUserName.Font = new Font("Segoe UI", 12.6667F, FontStyle.Bold, GraphicsUnit.Pixel);
            lblUserName.Location = new Point(62, 13);
            lblUserName.Name = "lblUserName";
            lblUserName.Size = new Size(87, 17);
            lblUserName.TabIndex = 1;
            lblUserName.Text = "Alex Morgan";
            // 
            // lblUserRole
            // 
            lblUserRole.AutoSize = true;
            lblUserRole.BackColor = Color.Transparent;
            lblUserRole.Font = new Font("Segoe UI", 11.3333F, FontStyle.Regular, GraphicsUnit.Pixel);
            lblUserRole.ForeColor = Color.FromArgb(107, 107, 107);
            lblUserRole.Location = new Point(62, 33);
            lblUserRole.Name = "lblUserRole";
            lblUserRole.Size = new Size(107, 15);
            lblUserRole.TabIndex = 2;
            lblUserRole.Text = "Inventory Manager";
            // 
            // Form1
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(245, 245, 244);
            ClientSize = new Size(1194, 834);
            Controls.Add(pnlMain);
            Controls.Add(pnlSidebar);
            DoubleBuffered = true;
            Font = new Font("Segoe UI", 12.6667F, FontStyle.Regular, GraphicsUnit.Pixel);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            MaximizeBox = false;
            Name = "Form1";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Stockroom - Inventory Catalog";
            pnlSidebar.ResumeLayout(false);
            pnlSidebar.PerformLayout();
            pnlMain.ResumeLayout(false);
            pnlContent.ResumeLayout(false);
            tlpContent.ResumeLayout(false);
            pnlHeader.ResumeLayout(false);
            pnlHeader.PerformLayout();
            tlpKpis.ResumeLayout(false);
            cardTable.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)dgv).EndInit();
            pnlPager.ResumeLayout(false);
            pnlPager.PerformLayout();
            pnlToolbar.ResumeLayout(false);
            pnlToolbar.PerformLayout();
            flpTabs.ResumeLayout(false);
            pnlTopBar.ResumeLayout(false);
            pnlSearch.ResumeLayout(false);
            pnlSearch.PerformLayout();
            pnlProfile.ResumeLayout(false);
            pnlProfile.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel pnlSidebar;
        private Badge badgeLogo;
        private Label lblBrand;
        private NavButton navDashboard;
        private NavButton navCatalog;
        private NavButton navSuppliers;
        private NavButton navAudit;
        private Panel pnlMain;
        private Panel pnlContent;
        private TableLayoutPanel tlpContent;
        private Panel pnlHeader;
        private Label lblTitle;
        private Label lblSubtitle;
        private OutlineButton btnImport;
        private OutlineButton btnExport;
        private TableLayoutPanel tlpKpis;
        private KpiCard kpiCapacity;
        private KpiCard kpiValue;
        private KpiCard kpiLow;
        private KpiCard kpiOut;
        private RoundedPanel cardTable;
        private DataGridView dgv;
        private DataGridViewTextBoxColumn colProduct;
        private DataGridViewTextBoxColumn colCategory;
        private DataGridViewTextBoxColumn colCost;
        private DataGridViewTextBoxColumn colPrice;
        private DataGridViewTextBoxColumn colMargin;
        private DataGridViewTextBoxColumn colStock;
        private DataGridViewTextBoxColumn colWarehouses;
        private DataGridViewTextBoxColumn colStatus;
        private DataGridViewTextBoxColumn colAction;
        private Panel pnlPager;
        private Label lblPageInfo;
        private OutlineButton btnPrev;
        private OutlineButton btnNext;
        private Panel pnlToolbar;
        private FlowLayoutPanel flpTabs;
        private TabButton btnTabAll;
        private TabButton btnTabHealthy;
        private TabButton btnTabLow;
        private TabButton btnTabOut;
        private TabButton btnTabOverstock;
        private ComboBox cboCategory;
        private ComboBox cboWarehouse;
        private Panel pnlTopBar;
        private RoundedPanel pnlSearch;
        private TextBox txtSearch;
        private RoundButton btnAddProduct;
        private OutlineButton btnBell;
        private Panel pnlProfile;
        private Badge badgeAvatar;
        private Label lblUserName;
        private Label lblUserRole;
    }
}