using System.Data.SqlClient;
using System.Threading.Tasks;

namespace handshake.Interfaces
{
  /// <summary>
  /// The <see cref="IAuthService"/> interface represents the authentication service.
  /// </summary>
  public interface IAuthService
  {
    /// <summary>
    /// Performs the Authentication and retrives a connection.
    /// </summary>
    /// <param name="username">The SQL username.</param>
    /// <param name="password">The SQL password.</param>
    /// <returns>An open connection.</returns>
    Task<SqlConnection> Authenticate(string username, string password);

    /// <summary>
    /// Retrives the current connection.
    /// </summary>
    /// <returns>the current connection.</returns>
    SqlConnection GetConnection();
  }
}
