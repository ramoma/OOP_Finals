#pragma warning disable WFO1000 // designer-serialization analyzer
using System.ComponentModel;
using System.Drawing.Drawing2D;
using System.Globalization;

namespace OOPInventoryCatalog;

public partial class Form1 : Form
{
    const int PageSize = 8;
    const int ColProduct = 0, ColStock = 5, ColStatus = 7, ColAction = 8;
    static readonly CultureInfo Us = CultureInfo.GetCultureInfo("en-US");

    readonly List<Product> _all = SampleData.All();

    public Form1()
    {
        InitializeComponent();
        if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return; // keep the Designer light
        Shown += (_, _) => { dgv.ClearSelection(); dgv.CurrentCell = null; };

        foreach (var col in new[] { colCost, colPrice, colMargin })
            col.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleRight;
        typeof(DataGridView).GetProperty("DoubleBuffered", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)!.SetValue(dgv, true);

        cboCategory.Items.Add("All categories");
        cboCategory.Items.AddRange(_all.Select(p => p.Category).Distinct().OrderBy(c => c).Cast<object>().ToArray());
        cboCategory.SelectedIndex = 0;
        cboWarehouse.SelectedIndex = 0;

        FillStaticScreen();
    }

    // ---------- Border lines drawn on the panels ----------
    void pnlSidebar_Paint(object? s, PaintEventArgs e)
    {
        using var pen = new Pen(Theme.Line);
        e.Graphics.DrawLine(pen, pnlSidebar.Width - 1, 0, pnlSidebar.Width - 1, pnlSidebar.Height);
        e.Graphics.DrawLine(pen, 0, 64, pnlSidebar.Width, 64);
    }
    void pnlTopBar_Paint(object? s, PaintEventArgs e)
    {
        using var pen = new Pen(Theme.Line);
        e.Graphics.DrawLine(pen, 0, pnlTopBar.Height - 1, pnlTopBar.Width, pnlTopBar.Height - 1);
    }
    void pnlToolbar_Paint(object? s, PaintEventArgs e)
    {
        using var pen = new Pen(Theme.Line);
        e.Graphics.DrawLine(pen, 0, pnlToolbar.Height - 1, pnlToolbar.Width, pnlToolbar.Height - 1);
    }
    void pnlPager_Paint(object? s, PaintEventArgs e)
    {
        using var pen = new Pen(Theme.Line);
        e.Graphics.DrawLine(pen, 0, 0, pnlPager.Width, 0);
    }
    void pnlProfile_Paint(object? s, PaintEventArgs e)
    {
        using var b = new SolidBrush(Theme.Gold);
        e.Graphics.FillRectangle(b, 0, 10, 3, 44);
    }

    // Keep the hand cursor over the Restock / Edit buttons (hover feel only, nothing happens on click)
    void dgv_CellMouseEnter(object? s, DataGridViewCellEventArgs e) =>
        dgv.Cursor = e.RowIndex >= 0 && e.ColumnIndex == ColAction ? Cursors.Hand : Cursors.Default;

    // ---------- Custom cell painting ----------
    void dgv_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex is not (ColProduct or ColStock or ColStatus or ColAction)) return;
        if (dgv.Rows[e.RowIndex].Tag is not Product p) return;

        e.Paint(e.CellBounds, DataGridViewPaintParts.Background | DataGridViewPaintParts.Border | DataGridViewPaintParts.SelectionBackground);
        var g = e.Graphics!; var c = e.CellBounds; int cy = c.Top + c.Height / 2;
        var noPad = TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix | TextFormatFlags.EndEllipsis;

        switch (e.ColumnIndex)
        {
            case ColProduct:
                var th = new Rectangle(c.Left + 10, cy - 16, 32, 32);
                Theme.FillRound(g, th, 8, Theme.IconBg);
                TextRenderer.DrawText(g, p.Initials, Theme.SmallBold, th, Theme.Ink, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | noPad);
                TextRenderer.DrawText(g, p.Name, Theme.Bold, new Rectangle(c.Left + 52, cy - 19, c.Width - 58, 18), Theme.Ink, TextFormatFlags.Left | TextFormatFlags.Bottom | noPad);
                TextRenderer.DrawText(g, $"{p.Sku} · {p.Supplier}", Theme.Small, new Rectangle(c.Left + 52, cy, c.Width - 58, 16), Theme.Muted, TextFormatFlags.Left | TextFormatFlags.Top | noPad);
                break;

            case ColStock:
                TextRenderer.DrawText(g, p.Stock.ToString(), Theme.Bold, new Rectangle(c.Left + 6, c.Top, 34, c.Height), Theme.Ink, TextFormatFlags.Right | TextFormatFlags.VerticalCenter | noPad);
                var track = new Rectangle(c.Left + 48, cy - 4, Math.Min(72, Math.Max(20, c.Width - 60)), 8);
                Theme.FillRound(g, track, 4, Theme.Row);
                double pct = Math.Min(1.0, p.Stock / (2.0 * p.Reorder));
                if (pct > 0) Theme.FillRound(g, new Rectangle(track.X, track.Y, Math.Max(8, (int)(track.Width * pct)), 8), 4, Theme.BarColor(p.Status));
                using (var pen = new Pen(Theme.Ink, 2f)) g.DrawLine(pen, track.X + track.Width / 2, track.Y - 4, track.X + track.Width / 2, track.Bottom + 4); // reorder point
                break;

            case ColStatus:
                var (fill, text) = Theme.StatusColors(p.Status);
                var label = p.Status.ToString();
                int w = TextRenderer.MeasureText(label, Theme.SmallBold).Width + 20;
                var pill = new Rectangle(c.Left + 6, cy - 11, w, 22);
                Theme.FillRound(g, pill, 11, fill);
                TextRenderer.DrawText(g, label, Theme.SmallBold, pill, text, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | noPad);
                break;

            case ColAction:
                bool restock = p.Status is StockStatus.Low or StockStatus.Out;
                var btn = new Rectangle(c.Left + 6, cy - 15, 78, 30);
                if (restock) Theme.FillRound(g, btn, 10, Theme.Gold); else Theme.FillRound(g, btn, 10, Color.White, Theme.Line);
                TextRenderer.DrawText(g, restock ? "Restock" : "Edit", Theme.SmallBold, btn, Theme.Ink, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | noPad);
                break;
        }
        e.Handled = true;
    }

    // ---------- Fills the screen once with sample data (no filtering, paging or clicks) ----------
    void FillStaticScreen()
    {
        int units = _all.Sum(p => p.Stock);
        decimal value = _all.Sum(p => p.Stock * p.Cost);
        int low = _all.Count(p => p.Status == StockStatus.Low), outCount = _all.Count(p => p.Status == StockStatus.Out);
        string Items(int n) => $"{n} Item{(n == 1 ? "" : "s")}";

        kpiCapacity.Value = $"{_all.Count} SKUs"; kpiCapacity.Sub = $"{units:N0} units in stock";
        kpiValue.Value = value.ToString("C2", Us);
        kpiLow.Value = Items(low);
        kpiOut.Value = Items(outCount);

        btnTabAll.Text = $"All  {_all.Count}";
        btnTabHealthy.Text = $"Healthy  {_all.Count(p => p.Status == StockStatus.Healthy)}";
        btnTabLow.Text = $"Low  {low}";
        btnTabOut.Text = $"Out  {outCount}";
        btnTabOverstock.Text = $"Overstock  {_all.Count(p => p.Status == StockStatus.Overstock)}";

        foreach (var p in _all.Take(PageSize))
        {
            int i = dgv.Rows.Add(p.Name, p.Category, p.Cost.ToString("C2", Us), p.Price.ToString("C2", Us),
                p.Margin.ToString("P0", Us), p.Stock, $"{p.Wh1} · {p.Wh2} · {p.Wh3}", p.Status.ToString(),
                p.Status is StockStatus.Low or StockStatus.Out ? "Restock" : "Edit");
            dgv.Rows[i].Tag = p;
        }
        lblPageInfo.Text = $"Showing 1–{Math.Min(PageSize, _all.Count)} of {_all.Count} SKUs";
    }

    private void btnTabHealthy_Click(object sender, EventArgs e)
    {

    }
}

// ================= Models =================

enum StockStatus { Healthy, Low, Out, Overstock }

record Product(string Name, string Initials, string Sku, string Category, string Supplier,
               decimal Cost, decimal Price, int Reorder, int Wh1, int Wh2, int Wh3)
{
    public int Stock => Wh1 + Wh2 + Wh3;
    public int[] Warehouses => new[] { Wh1, Wh2, Wh3 };
    public decimal Margin => Price == 0 ? 0 : (Price - Cost) / Price;

    // 0 = Out, at/below reorder point = Low, above 3x reorder point = Overstock
    public StockStatus Status =>
        Stock == 0 ? StockStatus.Out :
        Stock <= Reorder ? StockStatus.Low :
        Stock > Reorder * 3 ? StockStatus.Overstock : StockStatus.Healthy;
}

static class SampleData
{
    // Placeholder data. Replace with your database / API call.
    public static List<Product> All() => new()
    {
        new("Smart Security Camera 4K", "SC", "ELE-CAM-4K", "Electronics", "Lumen Tech", 118m, 189m, 15, 4, 3, 1),
        new("Mechanical RGB Keyboard", "MK", "ELE-KBD-RGB", "Electronics", "Keyline Co.", 46m, 89m, 12, 0, 0, 0),
        new("Ergonomic Mesh Chair", "EC", "FUR-CHR-MSH", "Furniture", "Sitwell Furnishings", 142m, 249m, 10, 3, 2, 1),
        new("MacBook Pro 16\" M3 Max", "MB", "ELE-MBP-16", "Electronics", "Orchard Distribution", 2980m, 3499m, 8, 9, 5, 2),
        new("USB-C Dock 11-in-1", "UD", "ACC-DOK-11", "Accessories", "Lumen Tech", 38m, 79m, 20, 5, 3, 1),
        new("27\" 4K Monitor", "4M", "ELE-MON-27", "Electronics", "Orchard Distribution", 276m, 429m, 12, 18, 11, 5),
        new("Standing Desk Frame", "SD", "FUR-DSK-STD", "Furniture", "Sitwell Furnishings", 189m, 349m, 8, 12, 7, 3),
        new("Cable Organizer Kit", "CO", "OFF-CBL-KIT", "Office Supplies", "Paperline", 3.20m, 9m, 30, 70, 30, 20),
        new("Wireless Ergonomic Mouse", "WM", "ELE-MSE-ERG", "Electronics", "Keyline Co.", 24m, 49m, 15, 22, 12, 6),
        new("Aluminum Laptop Stand", "LS", "ACC-STD-ALU", "Accessories", "Lumen Tech", 17.50m, 39m, 10, 15, 9, 4),
        new("A4 Copy Paper (5 reams)", "CP", "OFF-PPR-A4", "Office Supplies", "Paperline", 21m, 34m, 20, 25, 12, 8),
        new("LED Desk Lamp", "DL", "FUR-LMP-LED", "Furniture", "Sitwell Furnishings", 22m, 45m, 12, 16, 9, 5),
    };
}

// ================= Custom controls & theme =================

static class Theme
{
    static Color H(string s) => ColorTranslator.FromHtml(s);

    public static readonly Color Gold = H("#F59E0B");
    public static readonly Color RedFill = H("#FFF1F2"), RedText = H("#BE123C");
    public static readonly Color YellowFill = H("#FFFBEB"), YellowText = H("#B45309");
    public static readonly Color GreenFill = H("#ECFDF5"), GreenText = H("#065F46");
    public static readonly Color IndigoFill = H("#EEF2FF"), IndigoText = H("#4338CA");
    public static readonly Color HoverNav = H("#097D85"), Core = H("#0B96A0"), IconBg = H("#E6F6F7");
    public static readonly Color Line = H("#D9D9D9");   // borders / dividers only
    public static readonly Color Muted = H("#6B6B6B");  // secondary text (D9D9D9 is too light to read)
    public static readonly Color Row = H("#ECECEA");
    public static readonly Color Bg = H("#F5F5F4"), Ink = H("#1E1E1E");

    public static readonly string Family = "Segoe UI";
    public static Font F(float size, FontStyle style = FontStyle.Regular) => new(Family, size * 96f / 72f, style, GraphicsUnit.Pixel);
    public static readonly Font Body = F(9.5f), Bold = F(9.5f, FontStyle.Bold),
        Small = F(8.5f), SmallBold = F(8.5f, FontStyle.Bold), Title = F(18f, FontStyle.Bold), Kpi = F(18f, FontStyle.Bold);

    public static (Color Fill, Color Text) StatusColors(StockStatus s) => s switch
    {
        StockStatus.Healthy => (GreenFill, GreenText),
        StockStatus.Low => (YellowFill, YellowText),
        StockStatus.Out => (RedFill, RedText),
        _ => (IndigoFill, IndigoText),
    };
    public static Color BarColor(StockStatus s) => s switch
    {
        StockStatus.Healthy => Core,
        StockStatus.Low => Gold,
        StockStatus.Out => RedText,
        _ => IndigoText,
    };

    public static GraphicsPath Round(Rectangle r, int radius)
    {
        int d = Math.Min(radius * 2, Math.Min(r.Width, r.Height));
        var p = new GraphicsPath();
        if (d <= 0) { p.AddRectangle(r); return p; }
        p.AddArc(r.X, r.Y, d, d, 180, 90);
        p.AddArc(r.Right - d, r.Y, d, d, 270, 90);
        p.AddArc(r.Right - d, r.Bottom - d, d, d, 0, 90);
        p.AddArc(r.X, r.Bottom - d, d, d, 90, 90);
        p.CloseFigure();
        return p;
    }

    public static void FillRound(Graphics g, Rectangle r, int radius, Color fill, Color? border = null)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var path = Round(r, radius);
        if (fill.A > 0) { using var b = new SolidBrush(fill); g.FillPath(b, path); }
        if (border is Color c) { using var pen = new Pen(c); g.DrawPath(pen, path); }
    }
}

static class Icons
{
    public static void Draw(Graphics g, string name, Rectangle r, Color c)
    {
        g.SmoothingMode = SmoothingMode.AntiAlias;
        using var pen = new Pen(c, 1.6f) { LineJoin = LineJoin.Round, StartCap = LineCap.Round, EndCap = LineCap.Round };
        float x = r.X, y = r.Y, w = r.Width, h = r.Height, mx = x + w / 2;
        switch (name)
        {
            case "grid":
                g.DrawRectangle(pen, x, y, w * .42f, h * .42f); g.DrawRectangle(pen, x + w * .58f, y, w * .42f, h * .42f);
                g.DrawRectangle(pen, x, y + h * .58f, w * .42f, h * .42f); g.DrawRectangle(pen, x + w * .58f, y + h * .58f, w * .42f, h * .42f);
                break;
            case "box":
                g.DrawPolygon(pen, new[] { new PointF(mx, y), new PointF(x + w, y + h * .25f), new PointF(x + w, y + h * .75f), new PointF(mx, y + h), new PointF(x, y + h * .75f), new PointF(x, y + h * .25f) });
                g.DrawLine(pen, mx, y + h * .5f, mx, y + h);
                g.DrawLine(pen, mx, y + h * .5f, x, y + h * .25f); g.DrawLine(pen, mx, y + h * .5f, x + w, y + h * .25f);
                break;
            case "truck":
                g.DrawRectangle(pen, x, y + h * .2f, w * .6f, h * .5f); g.DrawRectangle(pen, x + w * .6f, y + h * .38f, w * .38f, h * .32f);
                g.DrawEllipse(pen, x + w * .1f, y + h * .66f, w * .2f, h * .2f); g.DrawEllipse(pen, x + w * .66f, y + h * .66f, w * .2f, h * .2f);
                break;
            case "log":
                g.DrawRectangle(pen, x + w * .15f, y, w * .7f, h);
                foreach (var f in new[] { .3f, .5f, .7f }) g.DrawLine(pen, x + w * .32f, y + h * f, x + w * .68f, y + h * f);
                break;
            case "layers":
                g.DrawPolygon(pen, new[] { new PointF(mx, y + h * .05f), new PointF(x + w, y + h * .3f), new PointF(mx, y + h * .55f), new PointF(x, y + h * .3f) });
                g.DrawLines(pen, new[] { new PointF(x, y + h * .52f), new PointF(mx, y + h * .77f), new PointF(x + w, y + h * .52f) });
                g.DrawLines(pen, new[] { new PointF(x, y + h * .74f), new PointF(mx, y + h * .99f), new PointF(x + w, y + h * .74f) });
                break;
            case "dollar":
                TextRenderer.DrawText(g, "$", Theme.F(11f, FontStyle.Bold), r, c, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
                break;
            case "warn":
                g.DrawPolygon(pen, new[] { new PointF(mx, y), new PointF(x + w, y + h), new PointF(x, y + h) });
                g.DrawLine(pen, mx, y + h * .4f, mx, y + h * .65f); g.DrawLine(pen, mx, y + h * .82f, mx, y + h * .83f);
                break;
            case "x":
                g.DrawEllipse(pen, x, y, w, h);
                g.DrawLine(pen, x + w * .32f, y + h * .32f, x + w * .68f, y + h * .68f); g.DrawLine(pen, x + w * .68f, y + h * .32f, x + w * .32f, y + h * .68f);
                break;
            case "bell":
                g.DrawArc(pen, x + w * .2f, y + h * .05f, w * .6f, h * .6f, 180, 180);
                g.DrawLine(pen, x + w * .2f, y + h * .35f, x + w * .2f, y + h * .72f); g.DrawLine(pen, x + w * .8f, y + h * .35f, x + w * .8f, y + h * .72f);
                g.DrawLine(pen, x + w * .05f, y + h * .76f, x + w * .95f, y + h * .76f);
                g.DrawArc(pen, x + w * .4f, y + h * .78f, w * .2f, h * .2f, 0, 180);
                break;
        }
    }
}

class RoundedPanel : Panel
{
    public int Radius { get; set; } = 16;
    public Color Fill { get; set; } = Color.White;
    public Color Border { get; set; } = Theme.Line;
    public RoundedPanel() { DoubleBuffered = true; ResizeRedraw = true; }
    protected override void OnPaintBackground(PaintEventArgs e) => e.Graphics.Clear(Parent?.BackColor ?? Theme.Bg);
    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        Theme.FillRound(e.Graphics, new Rectangle(0, 0, Width - 1, Height - 1), Radius, Fill, Border);
    }
    protected override void OnResize(EventArgs e)
    {
        base.OnResize(e);
        if (Width > 0 && Height > 0) { using var p = Theme.Round(new Rectangle(0, 0, Width, Height), Radius); Region = new Region(p); }
    }
}

class Badge : Control
{
    public Color Fill { get; set; } = Theme.Line;
    public int Radius { get; set; } = 10;
    public Badge() { SetStyle(ControlStyles.SupportsTransparentBackColor | ControlStyles.OptimizedDoubleBuffer, true); BackColor = Color.Transparent; Font = Theme.Bold; }
    protected override void OnPaint(PaintEventArgs e)
    {
        e.Graphics.Clear(Parent?.BackColor ?? Theme.Bg);
        Theme.FillRound(e.Graphics, new Rectangle(0, 0, Width - 1, Height - 1), Radius, Fill);
        TextRenderer.DrawText(e.Graphics, Text, Font, ClientRectangle, Theme.Ink, TextFormatFlags.HorizontalCenter | TextFormatFlags.VerticalCenter | TextFormatFlags.NoPadding);
    }
}

class RoundButton : Button
{
    public Color FillColor { get; set; } = Theme.Core;
    public Color HoverFill { get; set; } = Theme.HoverNav;
    public Color TextColor { get; set; } = Theme.Ink;
    public Color HoverText { get; set; } = Color.White;
    public Color SelectedFill { get; set; } = Theme.Core;
    public Color SelectedText { get; set; } = Theme.Ink;
    public Color BorderColor { get; set; } = Color.Transparent;
    public string IconName { get; set; } = "";
    public int Radius { get; set; } = 12;
    public bool Selected { get => _sel; set { _sel = value; Invalidate(); } }
    bool _hover, _sel;

    public RoundButton()
    {
        FlatStyle = FlatStyle.Flat; FlatAppearance.BorderSize = 0; UseVisualStyleBackColor = false;
        Cursor = Cursors.Hand; Font = Theme.Bold; Height = 40;
        SetStyle(ControlStyles.UserPaint | ControlStyles.AllPaintingInWmPaint | ControlStyles.OptimizedDoubleBuffer, true);
    }

    protected override void OnPaintBackground(PaintEventArgs e) { }
    protected override void OnMouseEnter(EventArgs e) { _hover = true; Invalidate(); base.OnMouseEnter(e); }
    protected override void OnMouseLeave(EventArgs e) { _hover = false; Invalidate(); base.OnMouseLeave(e); }

    protected override void OnPaint(PaintEventArgs e)
    {
        var g = e.Graphics;
        g.Clear(Parent?.BackColor ?? Theme.Bg);
        bool hot = _hover && Enabled;
        Color fill = _sel ? SelectedFill : hot ? HoverFill : FillColor;
        Color txt = !Enabled ? Theme.Muted : _sel ? SelectedText : hot ? HoverText : TextColor;
        Color? border = BorderColor.A == 0 ? null : BorderColor;
        Theme.FillRound(g, new Rectangle(0, 0, Width - 1, Height - 1), Radius, fill, border);

        var flags = TextFormatFlags.VerticalCenter | TextFormatFlags.EndEllipsis | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
        if (IconName != "" && string.IsNullOrEmpty(Text))
        {
            Icons.Draw(g, IconName, new Rectangle((Width - 20) / 2, (Height - 20) / 2, 20, 20), txt);
        }
        else if (IconName != "")
        {
            var circle = new Rectangle(8, (Height - 32) / 2, 32, 32);
            using (var b = new SolidBrush(Theme.IconBg)) g.FillEllipse(b, circle);
            Icons.Draw(g, IconName, Rectangle.Inflate(circle, -8, -8), Theme.HoverNav);
            TextRenderer.DrawText(g, Text, Font, new Rectangle(52, 0, Width - 60, Height), txt, flags | TextFormatFlags.Left);
        }
        else
        {
            TextRenderer.DrawText(g, Text, Font, ClientRectangle, txt, flags | TextFormatFlags.HorizontalCenter);
        }
        if (Focused && ShowFocusCues) ControlPaint.DrawFocusRectangle(g, Rectangle.Inflate(ClientRectangle, -3, -3));
    }
}

class KpiCard : RoundedPanel
{
    public string Title { get; set; } = "";
    public string Value { get; set; } = "";
    public string Sub { get; set; } = "";
    public string IconName { get; set; } = "";
    public Color IconBg { get; set; } = Theme.IconBg;
    public Color IconFg { get; set; } = Theme.HoverNav;
    public Color SubColor { get; set; } = Theme.Muted;
    public bool Link { get; set; }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        var g = e.Graphics;
        var circle = new Rectangle(Width - 20 - 32, 12, 32, 32);
        g.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
        using (var b = new SolidBrush(IconBg)) g.FillEllipse(b, circle);
        Icons.Draw(g, IconName, Rectangle.Inflate(circle, -8, -8), IconFg);
        var f = TextFormatFlags.Left | TextFormatFlags.NoPadding | TextFormatFlags.NoPrefix;
        TextRenderer.DrawText(g, Title, Theme.Bold, new Point(20, 18), Theme.Muted, f);
        TextRenderer.DrawText(g, Value, Theme.Kpi, new Point(20, 40), Theme.Ink, f);
        TextRenderer.DrawText(g, Sub, Link ? Theme.SmallBold : Theme.Small, new Point(20, 74), SubColor, f);
    }
}

// Pre-styled button flavours so they show up in the Toolbox and need no setup in the Designer
class NavButton : RoundButton
{
    public NavButton()
    {
        Radius = 22; FillColor = Color.Transparent; HoverFill = Theme.HoverNav; HoverText = Color.White;
        SelectedFill = Theme.Core; SelectedText = Theme.Ink; Size = new Size(192, 44);
    }
}

class TabButton : RoundButton
{
    public TabButton()
    {
        Radius = 20; FillColor = Color.Transparent; TextColor = Theme.Ink; HoverFill = Theme.IconBg; HoverText = Theme.Ink;
        SelectedFill = Theme.Ink; SelectedText = Color.White; Size = new Size(100, 40);
    }
}

class OutlineButton : RoundButton
{
    public OutlineButton()
    {
        FillColor = Color.White; BorderColor = Theme.Line; HoverFill = Theme.IconBg; HoverText = Theme.Ink;
    }
}