using System;
using System.Collections.Generic;

namespace handshake.GetData
{
  /// <summary>
  /// The detailed post information.
  /// </summary>
  public class PostDetailGetData
  {
    /// <summary>
    /// The id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The author.
    /// </summary>
    public Guid Author { get; set; }

    /// <summary>
    /// The author name.
    /// </summary>
    public string AuthorName { get; set; }

    /// <summary>
    /// The content.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// The creation date.
    /// </summary>
    public DateTime Creationdate { get; set; }

    /// <summary>
    /// The post reply.
    /// </summary>
    public List<PostReplyGetData> Replys { get; set; }
  }
}
