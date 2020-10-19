using handshake.Contexts;
using handshake.Data;
using handshake.Extensions;
using handshake.GetData;
using handshake.Interfaces;
using handshake.PutData;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Data.SqlClient;
using System.IO;
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
    private readonly FileRepository fileRepository;
    private readonly IAuthService userService;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="PostController"/> class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    /// <param name="userDatabaseAccess">The database access for the user.</param>
    /// <param name="fileRepository">The file repo</param>
    public ProfileController(IAuthService userService, UserDatabaseAccess userDatabaseAccess, FileRepository fileRepository)
    {
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
      this.fileRepository = fileRepository;
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
    /// Updates the users profile.
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

    /// <summary>
    /// Update the user avatar.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>The avatar info.</returns>
    [HttpPut("Avatar")]
    public async Task<ProfileGetData> AvatarPut(IFormFile file)
    {
      using SqlConnection connection = this.userService.Connection;
      var token = await this.fileRepository.UploadInternal("avatar" + Path.GetExtension(file.FileName),
                                                           file.OpenReadStream(),
                                                           connection,
                                                           true);

      using DatabaseContext context = new DatabaseContext(connection);
      Entities.UserEntity user = await context.ShakeUser.FirstAsync(o => o.Username == this.userService.Username);
      user.Avatar = token.Id;
      await context.SaveChangesAsync();
      connection.Close();

      var result = new ProfileGetData();
      result.CopyPropertiesFrom(user);
      result.Avatar = token.GetUrl();

      return result;
    }

    #endregion Methods
  }
}