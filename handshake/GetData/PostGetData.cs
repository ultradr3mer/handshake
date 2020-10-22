using handshake.Data;
using System;

namespace handshake.GetData
{
  /// <summary>
  /// A post.
  /// </summary>
  public class PostGetData
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
    /// The avatar url.
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
    /// The image url.
    /// </summary>
    public string Image { get; set; }

    /// <summary>
    /// The count of replys.
    /// </summary>
    public int ReplyCount { get; set; }

    /// <summary>
    /// The timespan since posted.
    /// </summary>
    public SimpleTimeSpan TimeAgo { get; set; }

    #endregion Properties
  }
}