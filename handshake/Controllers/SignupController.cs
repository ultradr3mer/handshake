using handshake.Contexts;
using handshake.Entities;
using handshake.Extensions;
using handshake.Interfaces;
using handshake.PostData;
using handshake.Repositories;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="SignupController"/> provides functionality to signup a new user.
  /// </summary>
  [Route("[controller]")]
  [ApiController]
  public class SignupController : ControllerBase
  {
    #region Fields

    private static readonly Regex alphanumericRegex = new Regex("^[a-zA-Z0-9]*$", RegexOptions.Compiled);
    private static readonly Regex base10DigitRegex = new Regex("[0-9]", RegexOptions.Compiled);
    private static readonly Regex latinLowercaseRegex = new Regex("[a-z]", RegexOptions.Compiled);
    private static readonly Regex latinUppercaseRegex = new Regex("[A-Z]", RegexOptions.Compiled);
    private static readonly Regex nonAlphanumericRegex = new Regex("[^a-zA-Z0-9]", RegexOptions.Compiled);

    private readonly IConfiguration configuration;
    private readonly IAuthService userService;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="SignupController"/> class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    /// <param name="configuration">The configuration.</param>
    public SignupController(IAuthService userService, IConfiguration configuration)
    {
      this.userService = userService;
      this.configuration = configuration;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Creates a new user.
    /// </summary>
    /// <param name="daten">The <see cref="UserPostData"/> to create.</param>
    /// <returns><see cref="OkResult"/>, when the user was created.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] UserPostData daten)
    {
      ValidateUsername(daten);
      ValidatePassword(daten);

      string username = this.configuration["AdminUsername"];
      string password = this.configuration["AdminPassword"];

      await this.userService.Authenticate(username, password);
      using (SqlConnection connection = this.userService.Connection)
      {
        DatabaseContext context = new DatabaseContext(connection);
        await CheckIfInviteIsValid(context, daten.InviteCode);

        connection.Close();
      }

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

        DatabaseContext context = new DatabaseContext(connection);
        var user = await CreateUserProfile(daten, context);
        await context.SaveChangesAsync();
        await LinkInviteCode(user.Id, daten.InviteCode, context);
        await context.SaveChangesAsync();

        connection.Close();
      }

      return this.Ok();
    }

    private async Task CheckIfInviteIsValid(DatabaseContext context, Guid inviteCode)
    {
      var invite = await context.Invite.FindAsync(inviteCode);

      if (invite == null)
      {
        throw new Exception("The invite does not exist.");
      }

      if (invite.UsedBy != null)
      {
        throw new Exception("This Invite has allready been used.");
      }
    }

    private async Task LinkInviteCode(Guid userId, Guid inviteCode, DatabaseContext context)
    {
      var invite = await context.Invite.FindAsync(inviteCode);
      invite.UsedBy = userId;
      context.Update(invite);
    }

    private static async Task<UserEntity> CreateUserProfile(UserPostData daten, DatabaseContext context)
    {
      UserEntity newUser = new UserEntity();
      newUser.CopyPropertiesFrom(daten);
      newUser.Nickname = newUser.Nickname ?? newUser.Username;
      await context.ShakeUser.AddAsync(newUser);
      return newUser;
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