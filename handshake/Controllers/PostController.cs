using handshake.Contexts;
using handshake.Data;
using handshake.Entities;
using handshake.Extensions;
using handshake.GetData;
using handshake.Interfaces;
using handshake.PostData;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

    private readonly UserDatabaseAccess userDatabaseAccess;
    private readonly IAuthService userService;

    #endregion Fields

    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="PostController"/> class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    /// <param name="userDatabaseAccess">The database access for the user.</param>
    public PostController(IAuthService userService, UserDatabaseAccess userDatabaseAccess)
    {
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
    }

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Gets a post by its id.
    /// </summary>
    /// <param name="Id">The id of the post to get.</param>
    /// <returns>The detailed post information.</returns>
    [HttpGet]
    public PostDetailGetData GetPost(Guid Id)
    {
      using var connection = this.userService.Connection;
      using var context = new DatabaseContext(connection);

      var now = DateTime.Now;

      var result = (from p in context.Post
                    join a in context.ShakeUser on p.Author equals a.Id
                    join f in context.FileAccessToken on a.Avatar equals f.Id into grouping
                    from f in grouping.DefaultIfEmpty()
                    where p.Id == Id
                    select new PostDetailGetData()
                    {
                      Id = p.Id,
                      Author = a.Id,
                      AuthorName = a.Nickname,
                      Content = p.Content,
                      Creationdate = p.Creationdate,
                      TimeAgo = new SimpleTimeSpan(now - p.Creationdate),
                      Avatar = FileTokenData.CreateUrl(f)
                    }).First();

      var replys = (from r in context.Reply
                    join a in context.ShakeUser on r.Author equals a.Id
                    join f in context.FileAccessToken on a.Avatar equals f.Id into grouping
                    from f in grouping.DefaultIfEmpty()
                    where r.Post == Id
                    select new PostReplyGetData()
                    {
                      Id = r.Id,
                      Author = a.Id,
                      AuthorName = a.Nickname,
                      Content = r.Content,
                      Creationdate = r.Creationdate,
                      TimeAgo = new SimpleTimeSpan(now - r.Creationdate),
                      Avatar = FileTokenData.CreateUrl(f)
                    }).ToList();

      result.Replys = replys;

      return result;
    }

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

      using var connection = this.userService.Connection;
      using var context = new DatabaseContext(connection);

      var now = DateTime.Now;

      var nowParameterName = "@NOW";
      var commandString = $@"SELECT CONTENT
                            ,USERID
	                          ,NICKNAME
	                          ,CREATIONDATE
	                          ,POSTID
	                          ,REPLYCOUNT
	                          ,TOKEN
	                          ,FILENAME
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
		                          ,FILEACCESSTOKEN.TOKEN
		                          ,FILEACCESSTOKEN.FILENAME
	                          FROM POST
	                          JOIN SHAKEUSER ON SHAKEUSER.ID = POST.AUTHOR
	                          LEFT OUTER JOIN FILEACCESSTOKEN ON FILEACCESSTOKEN.ID = SHAKEUSER.AVATAR 
	                          ) DATA
                          ORDER BY RELEVANCE";

      using var command = new SqlCommand(commandString, connection);
      command.Parameters.Add(nowParameterName, SqlDbType.DateTime);
      command.Parameters[nowParameterName].Value = now;

      using var reader = command.ExecuteReader();

      var result = new List<PostGetData>();

      while(await reader.ReadAsync())
      {
        object token = reader[6];
        object filename = reader[7];
        string avatar = string.Empty;
        if(!(token is System.DBNull))
        {
          avatar = FileTokenData.CreateUrl((long)token, (string)filename);
        }

        result.Add(new PostGetData()
        {
          Content = (string)reader[0],
          Author = (Guid)reader[1],
          AuthorName = (string)reader[2],
          Creationdate = (DateTime)reader[3],
          Id = (Guid)reader[4],
          ReplyCount = (int)reader[5],
          TimeAgo = new SimpleTimeSpan(now - (DateTime)reader[3]),
          Avatar = avatar
        });
      }

      return result;
    }

    /// <summary>
    /// Posts a new post.
    /// </summary>
    /// <param name="daten">The <see cref="PostPostData"/> to post.</param>
    /// <returns>The posted <see cref="PostEntity"/>.</returns>
    [HttpPost]
    public async Task<PostEntity> Post([FromBody] PostPostData daten)
    {
      using var connection = this.userService.Connection;
      var user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      var newPost = new PostEntity();
      newPost.CopyPropertiesFrom(daten);
      newPost.Creationdate = DateTime.Now;
      newPost.Author = user.Id;

      using var context = new DatabaseContext(connection);
      await context.Post.AddAsync(newPost);
      await context.SaveChangesAsync();
      return newPost;
    }

    #endregion Methods
  }
}