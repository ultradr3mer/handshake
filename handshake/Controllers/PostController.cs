using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using handshake.Contexts;
using handshake.Entities;
using handshake.ExtensionMethods;
using handshake.Interfaces;
using handshake.SetDaten;
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
    [Route("getcloseposts")]
    public IList<Post> GetClosePosts(decimal longitude, decimal latitude)
    {
      using (var connection = this.userService.GetConnection())
      {
        var context = new DatabaseContext(connection);
        return context.Post.ToList();
      }
    }

    /// <summary>
    /// Posts a new post.
    /// </summary>
    /// <param name="daten">The post to post.</param>
    /// <returns>The posted post.</returns>
    [HttpPost]
    public async Task<Post> Post([FromBody] InsertPostData daten)
    {
      using (var connection = this.userService.GetConnection())
      {
        var context = new DatabaseContext(connection);
        var newPost = new Post();
        newPost.CopyPropertiesFrom(daten);
        newPost.Creationdate = DateTime.Now;
        await context.Post.AddAsync(newPost);
        await context.SaveChangesAsync();
        return newPost;
      }
    }
  }
}
