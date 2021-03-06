﻿using handshake.Contexts;
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
using System;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
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

    private readonly FileRepository fileRepository;
    private readonly UserDatabaseAccess userDatabaseAccess;
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
    /// Gets the profile of the user with the given id or the current user.
    /// </summary>
    /// <param name="id">The id of the user.</param>
    /// <returns>The <see cref="ProfileGetData"/>.</returns>
    [HttpGet]
    public async Task<ProfileGetData> Get(Guid? id)
    {
      string name = null;
      if(id == null)
      {
        name = this.userService.Username;
      }

      using SqlConnection connection = this.userService.Connection;
      using DatabaseContext context = new DatabaseContext(connection);
      var profile = await (from s in context.ShakeUser
                           where (s.Username == name || name == null)
                           && (s.Id == id || id == null)
                           select new ProfileGetData
                           {
                             Groups = s.UserGroups.Select(g => new GroupGetData(g.Group)
                             {
                               OwnerName = g.Group.Owner.Nickname,
                               Icon = FileTokenData.CreateUrl(g.Group.Icon)
                             }).ToList(),
                             Avatar = FileTokenData.CreateUrl(s.Avatar)
                           }.CopyPropertiesFrom(s)).FirstAsync();

      return profile;
    }

    /// <summary>
    /// Updates the users profile.
    /// </summary>
    /// <param name="putData">The new <see cref="ProfilePutData"/>.</param>
    /// <returns>Ok, when the Profile was successfully updated./returns>
    [HttpPut]
    public async Task<IActionResult> Put(ProfilePutData putData)
    {
      using SqlConnection connection = this.userService.Connection;
      using DatabaseContext context = new DatabaseContext(connection);

      Entities.UserEntity user = await context.ShakeUser.FirstAsync(o => o.Username == this.userService.Username);
      user.CopyPropertiesFrom(putData);
      await context.SaveChangesAsync();
      connection.Close();

      return this.Ok();
    }

    /// <summary>
    /// Update the user avatar.
    /// </summary>
    /// <param name="file">The file.</param>
    /// <returns>The <see cref="FileUploadResultData"/>.</returns>
    [HttpPut("Avatar")]
    public async Task<FileUploadResultData> PutAvatar(IFormFile file)
    {
      using SqlConnection connection = this.userService.Connection;
      Entities.FileAccessTokenEntity token = await this.fileRepository.UploadInternal("avatar" + Path.GetExtension(file.FileName),
                                                                                       file.OpenReadStream(),
                                                                                       connection,
                                                                                       true);

      using DatabaseContext context = new DatabaseContext(connection);
      Entities.UserEntity user = await context.ShakeUser.FirstAsync(o => o.Username == this.userService.Username);
      user.Avatar = token;
      await context.SaveChangesAsync();
      connection.Close();

      return new FileUploadResultData(token);
    }

    #endregion Methods
  }
}