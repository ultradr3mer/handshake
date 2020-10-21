using System;
using System.ComponentModel.DataAnnotations;

namespace handshake.Entities
{
  /// <summary>
  /// A user.
  /// </summary>
  public class UserEntity
  {
    #region Properties

    /// <summary>
    /// The description.
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The users nickname.
    /// </summary>
    [MaxLength(200)]
    public string Nickname { get; set; }

    /// <summary>
    /// The username for login.
    /// </summary>
    [MaxLength(50)]
    public string Username { get; set; }

    /// <summary>
    /// The id of the avatar image.
    /// </summary>
    public FileAccessTokenEntity Avatar { get; set; }

    #endregion Properties
  }
}