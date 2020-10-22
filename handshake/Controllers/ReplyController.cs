using handshake.Contexts;
using handshake.Entities;
using handshake.Extensions;
using handshake.Interfaces;
using handshake.PostData;
using handshake.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace handshake.Controllers
{
  /// <summary>
  /// The <see cref="ReplyController"/> provides functionality to manage replys.
  /// </summary>
  [Authorize]
  [Route("[controller]")]
  [ApiController]
  public class ReplyController : ControllerBase
  {
    #region Fields

    private UserDatabaseAccess userDatabaseAccess;
    private IAuthService userService;

    #endregion Fields

    #region Constructors

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

    #endregion Constructors

    #region Methods

    /// <summary>
    /// Posts a new reply.
    /// </summary>
    /// <param name="daten">The <see cref="ReplyPostData"/> to post.</param>
    /// <returns>The posted <see cref="ReplyEntity"/> entity.</returns>
    [HttpPost]
    public async Task<PostPostResultData> Post([FromBody] ReplyPostData daten)
    {
      using var connection = this.userService.Connection;

      var newReply = await this.SavePostEntity(daten, connection);
      await this.UpdatePostReplyCount(daten, connection);

      connection.Close();

      var result = new PostPostResultData();
      result.CopyPropertiesFrom(newReply);

      return result;
    }

    private async Task<ReplyEntity> SavePostEntity(ReplyPostData daten, SqlConnection connection)
    {
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

    private async Task UpdatePostReplyCount(ReplyPostData daten, SqlConnection connection)
    {
      const string IdParameterName = "@ID";
      var commandString = @$"UPDATE POST
                            SET REPLYCOUNT = (
                                SELECT COUNT(*) FROM REPLY

                                WHERE REPLY.POST = {IdParameterName}
		                            )
                            WHERE POST.ID = {IdParameterName}";
      var command = new SqlCommand(commandString, connection);
      command.Parameters.Add(IdParameterName, SqlDbType.UniqueIdentifier);
      command.Parameters[IdParameterName].Value = daten.Post;
      await command.ExecuteNonQueryAsync();
    }

    #endregion Methods
  }
}