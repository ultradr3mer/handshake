using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace handshake.Entities
{
  /// <summary>
  /// A group.
  /// </summary>
  public class GroupEntity
  {
    #region Properties

    /// <summary>
    /// The description.
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; }

    /// <summary>
    /// The associated icon.
    /// </summary>
    public FileAccessTokenEntity Icon { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the group.
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; }

    /// <summary>
    /// The id of the owner.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// The the owner.
    /// </summary>
    public UserEntity Owner { get; set; }

    /// <summary>
    /// The users assiciated with this this group.
    /// </summary>
    public ICollection<UserGroupEntity> GroupUsers { get; set; }

    /// <summary>
    /// The posts assiciated with this this group.
    /// </summary>
    public ICollection<PostGroupEntity> GroupPosts { get; set; }

    #endregion Properties
  }
}