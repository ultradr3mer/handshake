using handshake.Const;
using handshake.Contexts;
using handshake.Data;
using handshake.Entities;
using handshake.Extensions;
using handshake.GetData;
using handshake.Interfaces;
using handshake.PostData;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="GroupController"/> provides functionality for groups.
  /// </summary>
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class GroupController : ControllerBase
  {
    #region Fields

    private readonly FileRepository fileRepository;
    private readonly UserDatabaseAccess userDatabaseAccess;
    private readonly IAuthService userService;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="GroupController"/> class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    /// <param name="userDatabaseAccess">The database access for the user.</param>
    /// <param name="fileRepository">The file repository.</param>
    public GroupController(IAuthService userService, UserDatabaseAccess userDatabaseAccess, FileRepository fileRepository)
    {
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
      this.fileRepository = fileRepository;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Associates the post to a group.
    /// </summary>
    /// <param name="groupId">The id of the group.</param>
    /// <param name="postId">The post id.</param>
    /// <returns>Ok, when successfull.</returns>
    [HttpPost("AssociatePost")]
    public async Task<IActionResult> AssociatePost(Guid groupId, Guid postId)
    {
      using SqlConnection connection = this.userService.Connection;
      UserEntity user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      DateTime now = DateTime.Now;

      using DatabaseContext context = new DatabaseContext(connection);
      PostEntity postEntity = await context.Post.FindAsync(postId);

      if (postEntity.AuthorId != user.Id)
      {
        throw new Exception("This post was made by another user");
      }

      PostGroupEntity postGroup = new PostGroupEntity()
      {
        CreationDate = now,
        GroupId = groupId,
        PostId = postId
      };

      await context.PostGroup.AddAsync(postGroup);
      await context.SaveChangesAsync();

      return this.Ok();
    }

    /// <summary>
    /// Associates the authenticated user to a group.
    /// </summary>
    /// <param name="groupId">The id of the group.</param>
    /// <returns>Ok, when successfull.</returns>
    [HttpPost("AssociateUser")]
    public async Task<IActionResult> AssociateUser(Guid groupId)
    {
      using SqlConnection connection = this.userService.Connection;
      UserEntity user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      DateTime now = DateTime.Now;

      UserGroupEntity userGroup = new UserGroupEntity()
      {
        CreationDate = now,
        GroupId = groupId,
        UserId = user.Id
      };

      using DatabaseContext context = new DatabaseContext(connection);
      await context.UserGroup.AddAsync(userGroup);
      await context.SaveChangesAsync();

      return this.Ok();
    }

    /// <summary>
    /// Dissociates the authenticated user from a group.
    /// </summary>
    /// <param name="groupId">The id of the group.</param>
    /// <returns>Ok, when successfull.</returns>
    [HttpPost("DissociateUser")]
    public async Task<IActionResult> DissociateUser(Guid groupId)
    {
      using SqlConnection connection = this.userService.Connection;
      UserEntity user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      using DatabaseContext context = new DatabaseContext(connection);
      var userGroup = await (from ug in context.UserGroup
                             where ug.GroupId == groupId
                             && ug.UserId == user.Id
                             select ug).FirstOrDefaultAsync();
      context.Remove(userGroup);
      await context.SaveChangesAsync();

      return this.Ok();
    }

    /// <summary>
    /// Gets a group.
    /// </summary>
    [HttpGet]
    public async Task<GroupGetData> GetGroup(Guid? id, string name)
    {
      using SqlConnection connection = this.userService.Connection;
      using DatabaseContext context = new DatabaseContext(connection);

      if(id == null && name == null)
      {
        throw new ArgumentException($"Either '{nameof(id)}' or name '{nameof(name)}' be specified.", nameof(id));
      }

      var now = System.DateTime.Now;

      GroupGetData result = await (from g in context.ShakeGroup
                                   where (g.Id == id || id == null)
                                   && (g.Name == name || name == null)
                                   select new GroupGetData(g)
                                   {
                                     OwnerName = g.Owner.Nickname,
                                     Icon = FileTokenData.CreateUrl(g.Icon),
                                     Users = g.GroupUsers.Select(u => new AssociatedUserData 
                                     { 
                                       Id = u.User.Id, 
                                       Name = u.User.Nickname 
                                     }).ToList(),
                                     Posts = g.GroupPosts.Select(p => new PostGetData(p.Post)
                                     {
                                       AuthorName = p.Post.Author.Nickname, 
                                       Avatar = FileTokenData.CreateUrl(p.Post.Author.Avatar),
                                       Groups = p.Post.PostGroups.Select(pg => new AssociatedGroupData(pg.Group)).ToList(),
                                       Image = FileTokenData.CreateUrl(p.Post.Image),
                                       TimeAgo = new SimpleTimeSpan(p.Post.Creationdate - now)
                                     }).ToList()
                                   }).FirstAsync();

      return result;
    }

    /// <summary>
    /// Gets all groups.
    /// </summary>
    [HttpGet("GetAllGroups")]
    public async Task<IList<GroupGetData>> GetGroups()
    {
      using SqlConnection connection = this.userService.Connection;
      using DatabaseContext context = new DatabaseContext(connection);

      List<GroupGetData> result = await (from g in context.ShakeGroup
                                         select new GroupGetData(g)
                                         {
                                           OwnerName = g.Owner.Nickname,
                                           Icon = FileTokenData.CreateUrl(g.Icon)
                                         }).ToListAsync();

      return result;
    }

    /// <summary>
    /// Posts a new group.
    /// </summary>
    [HttpPost]
    public async Task<GroupPostResultData> PostGroup(GroupPostData data)
    {
      if(!RegularExpressions.AlphanumericRegex.IsMatch(data.Name))
      {
        throw new ArgumentException("Group name must be alphanumeric.", nameof(GroupPostData.Name));
      }

      using SqlConnection connection = this.userService.Connection;
      UserEntity user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      GroupEntity newGroup = new GroupEntity
      {
        OwnerId = user.Id
      };

      newGroup.CopyPropertiesFrom(data);

      using DatabaseContext context = new DatabaseContext(connection);
      await context.ShakeGroup.AddAsync(newGroup);
      await context.SaveChangesAsync();

      connection.Close();

      return new GroupPostResultData { Id = newGroup.Id };
    }

    /// <summary>
    /// Update the group icon.
    /// </summary>
    /// <param name="id">The group id.</param>
    /// <param name="file">The file.</param>
    /// <returns>The <see cref="FileUploadResultData"/>.</returns>
    [HttpPost("Icon")]
    public async Task<FileUploadResultData> PostImage([FromForm] Guid id, IFormFile file)
    {
      using SqlConnection connection = this.userService.Connection;
      UserEntity user = await this.userDatabaseAccess.Get(this.userService.Username, connection);
      using DatabaseContext context = new DatabaseContext(connection);
      GroupEntity group = await context.ShakeGroup.FindAsync(id);

      if (group.OwnerId != user.Id)
      {
        throw new Exception("This post was made by another user.");
      }

      DateTime now = DateTime.Now;
      FileAccessTokenEntity token = await this.fileRepository.UploadInternal($"group_{now:yyyyMMdd'_'HHmmss}" + Path.GetExtension(file.FileName),
                                                           file.OpenReadStream(),
                                                           connection,
                                                           false);

      group.Icon = token;
      await context.SaveChangesAsync();
      connection.Close();

      return new FileUploadResultData(token);
    }

    #endregion Methods
  }
}