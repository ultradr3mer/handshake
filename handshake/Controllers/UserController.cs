using handshake.Contexts;
using handshake.Entities;
using handshake.ExtensionMethods;
using handshake.Extensions;
using handshake.GetData;
using handshake.Interfaces;
using handshake.PostData;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="UserController"/> provides functionality to manage users.
  /// </summary>
  [Authorize]
  [ApiController]
  [Route("[controller]")]
  public class UserController : ControllerBase
  {
    #region Fields

    private static readonly Regex alphanumericRegex = new Regex("^[a-zA-Z0-9]*$", RegexOptions.Compiled);
    private static readonly Regex base10DigitRegex = new Regex("[0-9]", RegexOptions.Compiled);
    private static readonly Regex latinLowercaseRegex = new Regex("[a-z]", RegexOptions.Compiled);
    private static readonly Regex latinUppercaseRegex = new Regex("[A-Z]", RegexOptions.Compiled);
    private static readonly Regex nonAlphanumericRegex = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);

    private readonly IAuthService userService;
    private readonly IConfiguration configuration;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    /// <param name="configuration">The configuration.</param>
    public UserController(IAuthService userService, IConfiguration configuration)
    {
      this.userService = userService;
      this.configuration = configuration;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Gets all users nearby.
    /// </summary>
    /// <returns>Close users.</returns>
    [HttpGet]
    [Route("GetCloseUsers")]
    public IList<UserEntity> GetCloseUsers(decimal longitude, decimal latitude)
    {
      using SqlConnection connection = this.userService.Connection;
      using DatabaseContext context = new DatabaseContext(connection);

      return context.User.ToList();
    }

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="daten">The user to create.</param>
    /// <returns>The created user.</returns>
    [AllowAnonymous]
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserPostData daten)
    {
      ValidateUsername(daten);
      ValidatePassword(daten);

      string username = this.configuration["AdminUsername"];
      string password = this.configuration["AdminPassword"];

      await this.userService.AuthenticateMaster(username, password);
      using (SqlConnection connection = this.userService.Connection)
      {
        SqlCommand commandCreateUser = new SqlCommand($"CREATE LOGIN {daten.Username} WITH PASSWORD = '{daten.Password.Replace("'", "''")}'", connection);
        await commandCreateUser.ExecuteNonQueryAsync();
        connection.Close();
      }

      await this.userService.Authenticate(username, password);
      using (SqlConnection connection = this.userService.Connection)
      {
        SqlCommand commandAddUser = new SqlCommand($"CREATE USER {daten.Username} FOR LOGIN {daten.Username} WITH DEFAULT_SCHEMA = dbo", connection);
        await commandAddUser.ExecuteNonQueryAsync();

        SqlCommand commandAddRole = new SqlCommand($"ALTER ROLE [DB_USER] ADD MEMBER {daten.Username}", connection);
        await commandAddRole.ExecuteNonQueryAsync();

        await CreateUserProfile(daten, connection);
        connection.Close();
      }

      return this.Ok();
    }

    private static async Task<DatabaseContext> CreateUserProfile(UserPostData daten, SqlConnection connection)
    {
      DatabaseContext context = new DatabaseContext(connection);

      UserEntity newUser = new UserEntity();
      newUser.CopyPropertiesFrom(daten);
      await context.User.AddAsync(newUser);
      await context.SaveChangesAsync();
      return context;
    }

    private static void ValidatePassword(UserPostData daten)
    {
      if (daten.Password.ContainsIgnoreCase(daten.Username))
      {
        throw new ArgumentException("The password must not contain the account name of the user.", nameof(UserPostData.Password));
      }

      if (daten.Password.Length < 8)
      {
        throw new ArgumentException("The password is at least eight characters long.", nameof(UserPostData.Password));
      }

      if (!latinUppercaseRegex.IsMatch(daten.Password))
      {
        throw new ArgumentException("The password must contain uppercase letters (A through Z).", nameof(UserPostData.Password));
      }

      if (!latinLowercaseRegex.IsMatch(daten.Password))
      {
        throw new ArgumentException("The password must contain uppercase letters (a through z).", nameof(UserPostData.Password));
      }

      if (!base10DigitRegex.IsMatch(daten.Password))
      {
        throw new ArgumentException("The password must contain digits (0 through 9).", nameof(UserPostData.Password));
      }

      if (!nonAlphanumericRegex.IsMatch(daten.Password))
      {
        throw new ArgumentException("The password must contain non-alphanumeric characters such as: exclamation point (!), dollar sign ($), number sign (#), or percent (%).", nameof(UserPostData.Password));
      }
    }

    private static void ValidateUsername(UserPostData daten)
    {
      if (!alphanumericRegex.IsMatch(daten.Username))
      {
        throw new ArgumentException("Username must be alphanumeric.", nameof(UserPostData.Username));
      }
    }

    #endregion Methods
  }
}