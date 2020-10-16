using System;

namespace handshake.GetData
{
  /// <summary>
  /// A user.
  /// </summary>
  public class ProfileGetData
  {
    #region Properties

    /// <summary>
    /// The description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The users nickname.
    /// </summary>
    public string Nickname { get; set; }

    /// <summary>
    /// The username for login.
    /// </summary>
    public string Username { get; set; }

    #endregion Properties
  }
}