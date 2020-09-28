using System.Data.SqlClient;
using System.Threading.Tasks;

namespace handshake.Services
{
  public interface IUserService
  {
    Task<string> Authenticate(string username, string password);

    SqlConnection GetConnection();
  }

  public class UserService : IUserService
  {
    private SqlConnection connection;

    public async Task<string> Authenticate(string username, string password)
    {
      var connectionString = $"Server=tcp:server2.database.windows.net,1433;Initial Catalog=handshake;Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

      this.connection = await Task.Run(() =>
      {
        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
      });

      return username;
    }

    public SqlConnection GetConnection()
    {
      return this.connection;
    }
  }
}
