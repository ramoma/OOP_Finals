using Controls;
using Model;
using View;
using ENVReader;

namespace OOP_Finals;

public partial class Form1 : Form
{
    private readonly Suppliers _suppliers = new();
    private SuppliersController? _suppliersController;

    public Form1()
    {
        InitializeComponent();

        EnvReader.Load("/home/ramouna/Desktop/OOP_Finals/OOP_Finals/.env");

        // Same placement you had in the designer file.
        _suppliers.Location = new Point(330, 10);
        _suppliers.Size = new Size(861, 500);
        Controls.Add(_suppliers);

        Load += Form1_Load;
    }

    private async void Form1_Load(object? sender, EventArgs e)
    {
        try
        {
            string cs = Database.ConnectionString;

            _suppliersController = new SuppliersController(
                _suppliers,
                new SqlServerPurchaseOrderRepository(cs),
                new SqlServerSupplierRepository(cs),
                Session.UserId);

            await _suppliersController.LoadAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Could not start the Suppliers screen",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }
}
