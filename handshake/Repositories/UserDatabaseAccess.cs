using handshake.Contexts;
using handshake.Entities;
using handshake.GetData;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Linq;
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
    /// <returns>The <see cref="ProfileGetData"/> for the user.</returns>
    public async Task<UserEntity> Get(string username, SqlConnection connection)
    {
      using DatabaseContext context = new DatabaseContext(connection);

      var result = await (from s in context.ShakeUser
                          where s.Username == username
                          select s).FirstAsync();

      return result;
    }

    #endregion Methods
  }
}