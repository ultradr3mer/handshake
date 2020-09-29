using handshake.Interfaces;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace handshake.Services
{

  /// <summary>
  /// The <see cref="IAuthService"/> class is a service for authentication.
  /// </summary>
  internal class AuthService : IAuthService
  {
    private SqlConnection connection;

    /// <summary>
    /// Performs the Authentication with the SQL Server and retrives a connection.
    /// </summary>
    /// <param name="username">The SQL username.</param>
    /// <param name="password">The SQL password.</param>
    /// <returns>An open connection.</returns>
    public async Task<SqlConnection> Authenticate(string username, string password)
    {
      if(this.connection != null)
      {
        this.connection.Dispose();
        this.connection = null;
      }

      var connectionString = $"Server=tcp:server2.database.windows.net,1433;Initial Catalog=handshake;Persist Security Info=False;User ID={username};Password={password};MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;";

      this.connection = await Task.Run(() =>
      {
        var connection = new SqlConnection(connectionString);
        connection.Open();
        return connection;
      });

      return connection;
    }

    /// <summary>
    /// Retrives the current connection.
    /// </summary>
    /// <returns>the current connection.</returns>
    public SqlConnection GetConnection()
    {
      return this.connection;
    }
  }
}
