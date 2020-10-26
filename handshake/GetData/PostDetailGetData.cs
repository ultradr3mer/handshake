using handshake.Data;
using System;
using System.Collections.Generic;

namespace handshake.GetData
{
  /// <summary>
  /// The detailed post information.
  /// </summary>
  public class PostDetailGetData
  {
    #region Properties

    /// <summary>
    /// The author.
    /// </summary>
    public Guid Author { get; set; }

    /// <summary>
    /// The author name.
    /// </summary>
    public string AuthorName { get; set; }

    /// <summary>
    /// The url of the Avatar.
    /// </summary>
    public string Avatar { get; set; }

    /// <summary>
    /// The content.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// The creation date.
    /// </summary>
    public DateTime Creationdate { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The post image.
    /// </summary>
    public string Image { get; internal set; }

    /// <summary>
    /// The time since the post creation.
    /// </summary>
    public SimpleTimeSpan TimeAgo { get; set; }

    /// <summary>
    /// The associated groups.
    /// </summary>
    public List<AssociatedGroupData> Groups { get; set; }

    /// <summary>
    /// The post reply.
    /// </summary>
    public List<PostReplyGetData> Replys { get; set; }

    #endregion Properties
  }
}