using System.Data.SqlClient;
namespace OOP_Finals;

public partial class Form1 : Form
{
    string databaseUrl = Environment.GetEnvironmentVariable("DB_URL");
    SqlConnection conn = new SqlConnection(@"databaseUrl");
    SqlCommand cmd = null;
    public Form1()
    {
        InitializeComponent();
            
    }
}
