using System;
using System.ComponentModel.DataAnnotations;

namespace handshake.Entities
{
  /// <summary>
  /// An invite.
  /// </summary>
  public class InviteEntity
  {
    #region Properties

    /// <summary>
    /// The Id of the user that generated the invite.
    /// </summary>
    public Guid GeneratedBy { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The Id of the user that used the invite.
    /// </summary>
    public Guid? UsedBy { get; set; }

    #endregion Properties
  }
}