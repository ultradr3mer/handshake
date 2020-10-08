using handshake.Contexts;
using handshake.Extensions;
using handshake.GetData;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace handshake.Repositories
{
  /// <summary>
  /// The <see cref="UserDatabaseAccess"/> class is the database access for users.
  /// </summary>
  public class UserDatabaseAccess
  {
    #region Methods

    /// <summary>
    /// Gets a user by its username.
    /// </summary>
    /// <param name="username">The login username of the user.</param>
    /// <param name="connection">The <see cref="SqlConnection"/> to use.</param>
    /// <returns>The <see cref="UserGetData"/> for the user.</returns>
    public async Task<UserGetData> Get(string username, SqlConnection connection)
    {
      using DatabaseContext context = new DatabaseContext(connection);

      Entities.UserEntity user = await context.ShakeUser.FirstAsync(o => o.Username == username);
      UserGetData result = new UserGetData();
      result.CopyPropertiesFrom(user);

      return result;
    }

    #endregion Methods
  }
}