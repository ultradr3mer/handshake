using System;

namespace handshake.Data
{
  /// <summary>
  /// The <see cref="AssociatedUserData"/> is an associated user.
  /// </summary>
  public class AssociatedUserData
  {
    #region Properties

    /// <summary>
    /// The id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the group.
    /// </summary>
    public string Name { get; set; }

    #endregion Properties
  }
}