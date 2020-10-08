using handshake.Contexts;
using handshake.Entities;
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

    private readonly IAuthService userService;
    private readonly UserDatabaseAccess userDatabaseAccess;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="UserController"/> class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    /// <param name="userDatabaseAccess">The <see cref="UserDatabaseAccess"/>.</param>
    public UserController(IAuthService userService, UserDatabaseAccess userDatabaseAccess)
    {
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
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

      return context.ShakeUser.ToList();
    }

    /// <summary>
    /// Gets the current User.
    /// </summary>
    /// <returns>The <see cref="UserEntity"/>.</returns>
    [HttpGet]
    public async Task<UserGetData> Get()
    {
      using SqlConnection connection = this.userService.Connection;
      var user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      return user;
    }

    #endregion Methods
  }
}