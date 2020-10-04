using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using handshake.Contexts;
using handshake.Entities;
using handshake.ExtensionMethods;
using handshake.Interfaces;
using handshake.PostData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace handshake.Controllers
{
  /// <summary>
  /// The post controller provides functionality to manage posts.
  /// </summary>
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class PostController : ControllerBase
  {
    private readonly IAuthService userService;

    /// <summary>
    /// Creates a new instance of the PostController class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    public PostController(IAuthService userService)
    {
      this.userService = userService;
    }

    /// <summary>
    /// Gets all posts nearby.
    /// </summary>
    /// <returns>The Posts nearby.</returns>
    [HttpGet]
    [Route("GetClosePosts")]
    public IList<GetData.PostGetData> GetClosePosts(decimal longitude, decimal latitude)
    {
      using var connection = this.userService.GetConnection();
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
    /// <param name="daten">The post to post.</param>
    /// <returns>The posted post.</returns>
    [HttpPost]
    public async Task<PostEntity> Post([FromBody] PostPostData daten)
    {
      using var connection = this.userService.GetConnection();
      using var context = new DatabaseContext(connection);

      var newPost = new PostEntity();
      newPost.CopyPropertiesFrom(daten);
      newPost.Creationdate = DateTime.Now;
      await context.Post.AddAsync(newPost);
      await context.SaveChangesAsync();
      return newPost;

    }
  }
}
