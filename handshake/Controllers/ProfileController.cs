using handshake.Contexts;
using handshake.Extensions;
using handshake.GetData;
using handshake.Interfaces;
using handshake.PutData;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="ProfileController"/> provides functions to manage the user profile.
  /// </summary>
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class ProfileController : ControllerBase
  {
    #region Fields

    private readonly UserDatabaseAccess userDatabaseAccess;
    private readonly IAuthService userService;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="PostController"/> class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    /// <param name="userDatabaseAccess">The database access for the user.</param>
    public ProfileController(IAuthService userService, UserDatabaseAccess userDatabaseAccess)
    {
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Gets the current profile.
    /// </summary>
    /// <returns>The <see cref="ProfileGetData"/>.</returns>
    [HttpGet]
    public async Task<ProfileGetData> Get()
    {
      using SqlConnection connection = this.userService.Connection;
      var user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      return user;
    }

    /// <summary>
    /// Updates the users profile
    /// </summary>
    /// <param name="putData">The new <see cref="ProfilePutData"/>.</param>
    /// <returns>The updated <see cref="ProfileGetData"/>./returns>
    [HttpPut]
    public async Task<ProfileGetData> Put(ProfilePutData putData)
    {
      using SqlConnection connection = this.userService.Connection;
      using DatabaseContext context = new DatabaseContext(connection);

      Entities.UserEntity user = await context.ShakeUser.FirstAsync(o => o.Username == this.userService.Username);
      user.CopyPropertiesFrom(putData);
      await context.SaveChangesAsync();
      connection.Close();

      var result = new ProfileGetData();
      result.CopyPropertiesFrom(user);

      return result;
    }

    #endregion Methods
  }
}