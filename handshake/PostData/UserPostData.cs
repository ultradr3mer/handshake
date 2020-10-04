using System.ComponentModel.DataAnnotations;

namespace handshake.PostData
{
  /// <summary>
  /// This class contains all information needed to create an account.
  /// </summary>
  public class UserPostData
  {
    /// <summary>
    /// The public nickname.
    /// </summary>
    [MaxLength(200)]
    public string Nickname { get; set; }

    /// <summary>
    /// The username for login.
    /// </summary>
    [MaxLength(50)]
    public string Username { get; set; }

    /// <summary>
    /// The password.
    /// </summary>
    [MaxLength(50)]
    public string Password { get; set; }
  }
}
