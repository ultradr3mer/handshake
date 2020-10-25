using System;
using System.ComponentModel.DataAnnotations;

namespace handshake.Entities
{
  /// <summary>
  /// The <see cref="UserGroupEntity"/> is a relation between a user and a group.
  /// </summary>
  public class UserGroupEntity
  {
    #region Properties

    /// <summary>
    /// The date/time this link was created.
    /// </summary>
    public DateTime CreationDate { get; set; }

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
    /// The id of the user.
    /// </summary>
    public Guid UserId { get; set; }

    #endregion Properties
  }
}