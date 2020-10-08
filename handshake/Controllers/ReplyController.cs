using System;
using System.Threading.Tasks;
using handshake.Contexts;
using handshake.Entities;
using handshake.Extensions;
using handshake.Interfaces;
using handshake.PostData;
using handshake.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="ReplyController"/> provides functionality to manage replys.
  /// </summary>
  [Route("[controller]")]
  [ApiController]
  public class ReplyController : ControllerBase
  {
    private IAuthService userService;
    private UserDatabaseAccess userDatabaseAccess;

    /// <summary>
    /// Creates a new instance of the <see cref="PostController"/> class.
    /// </summary>
    /// <param name="userService">The user / login service.</param>
    /// <param name="userDatabaseAccess">The database access for the user.</param>
    public ReplyController(IAuthService userService, UserDatabaseAccess userDatabaseAccess)
    {
      this.userService = userService;
      this.userDatabaseAccess = userDatabaseAccess;
    }

    /// <summary>
    /// Posts a new reply.
    /// </summary>
    /// <param name="daten">The <see cref="ReplyPostData"/> to post.</param>
    /// <returns>The posted <see cref="ReplyEntity"/> entity.</returns>
    [HttpPost]
    public async Task<ReplyEntity> Post([FromBody] ReplyPostData daten)
    {
      using var connection = this.userService.Connection;
      var user = await this.userDatabaseAccess.Get(this.userService.Username, connection);

      var newReply = new ReplyEntity();
      newReply.CopyPropertiesFrom(daten);
      newReply.Creationdate = DateTime.Now;
      newReply.Author = user.Id;

      using var context = new DatabaseContext(connection);
      await context.Reply.AddAsync(newReply);
      await context.SaveChangesAsync();
      return newReply;
    }
  }
}
