using handshake.Data;
using handshake.Entities;
using handshake.Extensions;
using System;
using System.Collections.Generic;

namespace handshake.GetData
{
  /// <summary>
  /// A post.
  /// </summary>
  public class PostGetData
  {
    #region Constructors

    /// <summary>
    /// Creates a new <see cref="PostGetData"/> instance.
    /// </summary>
    public PostGetData()
    {
    }

    /// <summary>
    /// Creates a new <see cref="PostGetData"/> based on the <see cref="PostEntity"/>.
    /// </summary>
    /// <param name="entity">The <see cref="PostEntity"/> to copy properties from.</param>
    public PostGetData(PostEntity entity)
    {
      this.CopyPropertiesFrom(entity);
    }

    #endregion Constructors

    #region Properties

    /// <summary>
    /// The author.
    /// </summary>
    public Guid AuthorId { get; set; }

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
    /// The associated groups.
    /// </summary>
    public List<AssociatedGroupData> Groups { get; set; }

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