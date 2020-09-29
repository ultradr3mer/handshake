using System.Data.SqlClient;
using System.Threading.Tasks;

namespace handshake.Services
{
  /// <summary>
  /// The <see cref="IAuthService"/> interface represents the authentication service.
  /// </summary>
  public interface IAuthService
  {
    Task<SqlConnection> Authenticate(string username, string password);

    SqlConnection GetConnection();
  }
}
