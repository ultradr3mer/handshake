using System;
using System.Collections;
using System.Collections.Generic;
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
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class PostController : ControllerBase
  {
    private readonly IUserService userService;

    public PostController(IUserService userService)
    {
      this.userService = userService;
    }

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

    public async Task<IActionResult> Post([FromBody] InsertPostDaten daten)
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
