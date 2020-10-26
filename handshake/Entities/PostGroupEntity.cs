using System;
using System.ComponentModel.DataAnnotations;

namespace handshake.Entities
{
  /// <summary>
  /// The <see cref="PostGroupEntity"/> is a relation between a post and a group.
  /// </summary>
  public class PostGroupEntity
  {
    #region Properties

    /// <summary>
    /// The date/time this link was created.
    /// </summary>
    public DateTime CreationDate { get; set; }

    /// <summary>
    /// The group.
    /// </summary>
    public GroupEntity Group { get; set; }

    /// <summary>
    /// The id of the group.
    /// </summary>
    public Guid GroupId { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The post.
    /// </summary>
    public PostEntity Post { get; set; }

    /// <summary>
    /// The id of the post.
    /// </summary>
    public Guid PostId { get; set; }

    #endregion Properties
  }
}