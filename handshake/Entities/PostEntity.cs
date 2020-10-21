using System;
using System.ComponentModel.DataAnnotations;

namespace handshake.Entities
{
  /// <summary>
  /// A post.
  /// </summary>
  public class PostEntity
  {
    #region Properties

    /// <summary>
    /// The author.
    /// </summary>
    public Guid Author { get; set; }

    /// <summary>
    /// The content.
    /// </summary>
    [MaxLength(1000)]
    public string Content { get; set; }

    /// <summary>
    /// The creation date.
    /// </summary>
    public DateTime Creationdate { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The location lattitude.
    /// </summary>
    public decimal Latitude { get; set; }

    /// <summary>
    /// The location longitude.
    /// </summary>
    public decimal Longitude { get; set; }

    /// <summary>
    /// The count of the post replys.
    /// </summary>
    public int? ReplyCount { get; set; }

    /// <summary>
    /// The image attached to the post.
    /// </summary>
    public FileAccessTokenEntity Image { get; set; }

    #endregion Properties
  }
}