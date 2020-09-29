using System.ComponentModel.DataAnnotations;

namespace handshake.Data
{
  /// <summary>
  /// The authentication model represents all data needed to sign in.
  /// </summary>
  public class AuthenticateModel
  {
    /// <summary>
    /// The login username.
    /// </summary>
    [Required]
    public string Username { get; set; }

    /// <summary>
    /// The login password.
    /// </summary>
    [Required]
    public string Password { get; set; }
  }
}
