using handshake.Data;
using System;

namespace handshake.GetData
{
  /// <summary>
  /// A reply.
  /// </summary>
  public class PostReplyGetData
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
    /// The content of this reply.
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
    /// The timme since the reply creation
    /// </summary>
    public SimpleTimeSpan TimeAgo { get; set; }

    #endregion Properties
  }
}