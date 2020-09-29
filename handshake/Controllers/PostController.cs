using System;
using System.Linq;
using System.Threading.Tasks;
using handshake.Contexts;
using handshake.Entities;
using handshake.ExtensionMethods;
using handshake.Services;
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
    /// <returns>Retruncode ok, when the retrival was a success.</returns>
    [HttpGet]
    [Route("getcloseposts")]
    public IActionResult GetClosePosts()
    {
      using (var connection = this.userService.GetConnection())
      {
        var context = new DatabaseContext(connection);
        return Ok(context.Post.ToList());
      }
    }

    /// <summary>
    /// Posts a new post.
    /// </summary>
    /// <param name="daten">The post to post.</param>
    /// <returns>Retruncode ok, whenn success.</returns>
    [HttpPost]
    public async Task<IActionResult> Post([FromBody] InsertPostData daten)
    {
      using (var connection = this.userService.GetConnection())
      {
        var context = new DatabaseContext(connection);
        var newPost = new Post();
        newPost.CopyPropertiesFrom(daten);
        newPost.Creationdate = DateTime.Now;
        await context.Post.AddAsync(newPost);
        await context.SaveChangesAsync();
        return Ok(newPost);
      }
    }
  }
}
