
public class DbService : IDbService
{
    readonly private string _connectionString;

    public DbService(IConfiguration config)
    {
        _connectionString = config.GetConnectionString("Default");
    }

   //Microsoft.Data.SqlClient
}