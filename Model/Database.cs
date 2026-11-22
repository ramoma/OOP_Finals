using ENVReader;

namespace Model;

/// <summary>Single place that knows where the connection string comes from.</summary>
public static class Database
{
    public static string ConnectionString
    {
        get
        {

            string? cs = Environment.GetEnvironmentVariable("DB_URL");

            if (string.IsNullOrWhiteSpace(cs))
                throw new InvalidOperationException(
                    "No database connection string found. Set DB_CONNECTION (in .env or as an environment variable).");

                return cs;
        }
    }
}

/// <summary>Who is logged in. Set UserId after your login succeeds (needed for the stock_movements audit log).</summary>
public static class Session
{
    /// <summary>users.id of the current user. For testing, paste the Guid of an existing row in the users table.</summary>
    public static Guid UserId { get; set; }
}
