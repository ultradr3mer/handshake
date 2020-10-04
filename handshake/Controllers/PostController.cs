using handshake.Contexts;
using handshake.Entities;
using handshake.ExtensionMethods;
using handshake.Interfaces;
using handshake.PostData;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
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
    /// Gets all posts nearby.
    /// </summary>
    /// <returns>The Posts nearby.</returns>
    [HttpGet]
    [Route("GetClosePosts")]
    public IList<GetData.PostGetData> GetClosePosts(decimal longitude, decimal latitude)
    {
      using var connection = this.userService.Connection;
      using var context = new DatabaseContext(connection);

      var result = (from post in context.Post
                    join author in context.User on post.Author equals author.Id
                    select new GetData.PostGetData()
                    {
                      Author = post.Author,
                      AuthorName = author.Nickname,
                      Content = post.Content,
                      Creationdate = post.Creationdate,
                      Id = post.Id
                    }).OrderBy(o => o.Creationdate).ToList();

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