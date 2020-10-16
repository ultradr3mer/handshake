using handshake.GetData;
using handshake.Interfaces;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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

    #endregion Methods
  }
}