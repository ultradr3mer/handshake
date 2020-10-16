using System.ComponentModel.DataAnnotations;

namespace handshake.PutData
{
  /// <summary>
  /// The data to update a user.
  /// </summary>
  public class ProfilePutData
  {
    #region Properties

    /// <summary>
    /// The description.
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; }

    /// <summary>
    /// The users nickname.
    /// </summary>
    [MaxLength(200)]
    public string Nickname { get; set; }

    #endregion Properties
  }
}