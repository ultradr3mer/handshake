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
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="PostController"/> provides functionality to manage posts.
  /// </summary>
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class PostController : ControllerBase
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
    /// <param name="fileRepository">The file repository.</param>
    public PostController(IAuthService userService, UserDatabaseAccess userDatabaseAccess, FileRepository fileRepository)
    {
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
      this.fileRepository = fileRepository;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Gets all posts nearby.
    /// </summary>
    /// <returns>The Posts nearby.</returns>
    [HttpGet]
    [Route("GetClosePosts")]
    public async Task<IList<GetData.PostGetData>> GetClosePosts(decimal? latitude, decimal? longitude)
    {
      if (latitude == null)
      {
        throw new ArgumentNullException(nameof(latitude));
      }

      if (longitude == null)
      {
        throw new ArgumentNullException(nameof(longitude));
      }

      using SqlConnection connection = this.userService.Connection;
      var result = await this.LoadClosePostEnitiesAsync(connection, latitude, longitude);
      result = await LoadGroups(connection, result);
      connection.Close();

      return result;
    }

    /// <summary>
    /// Gets a post by its id.
    /// </summary>
    /// <param name="Id">The id of the post to get.</param>
    /// <returns>The detailed post information.</returns>
    [HttpGet]
    public PostDetailGetData GetPost(Guid Id)
    {
      using SqlConnection connection = this.userService.Connection;
      using DatabaseContext context = new DatabaseContext(connection);

      DateTime now = DateTime.Now;

      PostDetailGetData result = (from p in context.Post
                                  join a in context.ShakeUser on p.AuthorId equals a.Id
                                  where p.Id == Id
                                  select new PostDetailGetData()
                                  {
                                    Id = p.Id,
                                    Author = a.Id,
                                    AuthorName = a.Nickname,
                                    Content = p.Content,
                                    Creationdate = p.Creationdate,
                                    TimeAgo = new SimpleTimeSpan(now - p.Creationdate),
                                    Avatar = FileTokenData.CreateUrl(a.Avatar),
                                    Image = FileTokenData.CreateUrl(p.Image),
                                    Groups = p.PostGroups.Select(o => new AssociatedGroupData(o.Group)).ToList()
                                  }).First();

      List<PostReplyGetData> replys = (from r in context.Reply
                                       join a in context.ShakeUser on r.Author equals a.Id
                                       where r.Post == Id
                                       select new PostReplyGetData()
                                       {
                                         Id = r.Id,
                                         Author = a.Id,
                                         AuthorName = a.Nickname,
                                         Content = r.Content,
                                         Creationdate = r.Creationdate,
                                         TimeAgo = new SimpleTimeSpan(now - r.Creationdate),
                                         Avatar = FileTokenData.CreateUrl(a.Avatar),
                                         Image = FileTokenData.CreateUrl(r.Image)
                                       }).OrderBy(o => o.Creationdate).ToList();

      result.Replys = replys;

      return result;
    }

    /// <summary>
    /// Posts a new post.
    /// </summary>
    /// <param name="daten">The <see cref="PostPostData"/> to post.</param>
    /// <returns>The posted <see cref="PostPostResultData"/>.</returns>
    [HttpPost]
    public async Task<PostPostResultData> Post([FromBody] PostPostData daten)
    {
      using SqlConnection connection = this.userService.Connection;
      UserEntity user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      DateTime now = DateTime.Now;

      PostEntity newPost = new PostEntity();
      newPost.CopyPropertiesFrom(daten);
      newPost.Creationdate = now;
      newPost.AuthorId = user.Id;

      using DatabaseContext context = new DatabaseContext(connection);
      await using var transaction = await context.Database.BeginTransactionAsync();

      await context.Post.AddAsync(newPost);
      await context.SaveChangesAsync();

      await CheckGroups(daten.Content, user.Id, newPost.Id, now, context);

      await transaction.CommitAsync();
      connection.Close();
      return new PostPostResultData() { Id = newPost.Id };
    }

    /// <summary>
    /// Update the post image.
    /// </summary>
    /// <param name="id">The post id.</param>
    /// <param name="file">The file.</param>
    /// <returns>The <see cref="FileUploadResultData"/>.</returns>
    [HttpPost("Image")]
    public async Task<FileUploadResultData> PostImage([FromForm] Guid id, IFormFile file)
    {
      using SqlConnection connection = this.userService.Connection;
      UserEntity user = await this.userDatabaseAccess.Get(this.userService.Username, connection);
      using DatabaseContext context = new DatabaseContext(connection);
      PostEntity targetPost = await context.Post.FindAsync(id);

      if (targetPost.AuthorId != user.Id)
      {
        throw new Exception("This post was made by another user.");
      }

      DateTime now = DateTime.Now;
      FileAccessTokenEntity token = await this.fileRepository.UploadInternal($"post_{now:yyyyMMdd'_'HHmmss}" + Path.GetExtension(file.FileName),
                                                           file.OpenReadStream(),
                                                           connection,
                                                           false);

      targetPost.Image = token;
      await context.SaveChangesAsync();
      connection.Close();

      return new FileUploadResultData(token);
    }

    private static async Task CheckGroups(string datenContent, Guid userId, Guid newPostId, DateTime now, DatabaseContext context)
    {
      var groupMatches = RegularExpressions.HashtagGroupRegex.Matches(datenContent);
      foreach (Match item in groupMatches)
      {
        var groupName = item.Groups["name"].Value;
        var groupId = await (from g in context.ShakeGroup
                             where g.Name == groupName
                             && g.GroupUsers.Any(o => o.UserId == userId)
                             select g.Id).FirstOrDefaultAsync();

        if (groupId == default)
        {
          throw new Exception($"The group {groupName} does not exist or you are not in it.");
        }

        await context.PostGroup.AddAsync(new PostGroupEntity { CreationDate = now, GroupId = groupId, PostId = newPostId });
        await context.SaveChangesAsync();
      }
    }

    private static async Task<List<PostGetData>> LoadGroups(SqlConnection connection, List<PostGetData> group)
    {
      var resultDictionary = group.ToDictionary(o => o.Id);
      var ids = resultDictionary.Keys.ToArray();
      DatabaseContext context = new DatabaseContext(connection);
      var groups = await (from pg in context.PostGroup
                          where ids.Contains(pg.PostId)
                          select new { pg.PostId, pg.Group }).ToListAsync();

      foreach (var item in groups)
      {
        resultDictionary[item.PostId].Groups.Add(new AssociatedGroupData(item.Group));
      }

      return group;
    }

    private async Task<List<PostGetData>> LoadClosePostEnitiesAsync(SqlConnection connection, decimal? latitude, decimal? longitude)
    {
      DateTime now = DateTime.Now;
      string nowParameterName = "@NOW";
      string commandString = $@"SELECT CONTENT
                            ,USERID
	                          ,NICKNAME
	                          ,CREATIONDATE
	                          ,POSTID
	                          ,REPLYCOUNT
	                          ,AVATARTOKEN
	                          ,AVATARFILENAME
                            ,IMAGETOKEN
                            ,IMAGEFILENAME
	                          ,DIST + AGO AS RELEVANCE
                          FROM (
	                          SELECT POST.CONTENT
		                          ,SHAKEUSER.ID AS USERID
		                          ,SHAKEUSER.NICKNAME
		                          ,POST.CREATIONDATE
		                          ,POST.ID AS POSTID
		                          ,COALESCE(POST.REPLYCOUNT,0) AS REPLYCOUNT
		                          ,DBO.DISTANCE(POST.LATITUDE, POST.LONGITUDE, {latitude}, {longitude}) AS DIST
		                          ,DATEDIFF(MINUTE, POST.CREATIONDATE, {nowParameterName}) AS AGO
		                          ,AVATAR.TOKEN AS AVATARTOKEN
		                          ,AVATAR.FILENAME AS AVATARFILENAME
		                          ,IMAGE.TOKEN AS IMAGETOKEN
		                          ,IMAGE.FILENAME AS IMAGEFILENAME
	                          FROM POST
	                          JOIN SHAKEUSER ON SHAKEUSER.ID = POST.AUTHOR
	                          LEFT OUTER JOIN FILEACCESSTOKEN AVATAR ON AVATAR.ID = SHAKEUSER.AVATARID
	                          LEFT OUTER JOIN FILEACCESSTOKEN IMAGE ON IMAGE.ID = POST.IMAGEID
	                          ) DATA
                          ORDER BY RELEVANCE";

      using SqlCommand command = new SqlCommand(commandString, connection);
      command.Parameters.Add(nowParameterName, SqlDbType.DateTime);
      command.Parameters[nowParameterName].Value = now;

      using SqlDataReader reader = command.ExecuteReader();

      List<PostGetData> result = new List<PostGetData>();

      while (await reader.ReadAsync())
      {
        result.Add(new PostGetData()
        {
          Content = (string)reader[0],
          AuthorId = (Guid)reader[1],
          AuthorName = (string)reader[2],
          Creationdate = (DateTime)reader[3],
          Id = (Guid)reader[4],
          ReplyCount = (int)reader[5],
          TimeAgo = new SimpleTimeSpan(now - (DateTime)reader[3]),
          Avatar = FileTokenData.CreateUrl(reader[6], reader[7]),
          Image = FileTokenData.CreateUrl(reader[8], reader[9]),
          Groups = new List<AssociatedGroupData>()
        });
      }

      return result;
    }

    #endregion Methods
  }
}