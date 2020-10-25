using System;

namespace handshake.GetData
{
  /// <summary>
  /// The <see cref="GroupGetData"/> contains information about the group.
  /// </summary>
  public class GroupGetData
  {
    #region Properties

    /// <summary>
    /// The description.
    /// </summary>
    public string Description { get; set; }

    /// <summary>
    /// The associated icon.
    /// </summary>
    public string Icon { get; set; }

    /// <summary>
    /// The id.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// The name of the group.
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// The id of the owner.
    /// </summary>
    public Guid OwnerId { get; set; }

    /// <summary>
    /// The name of the owner.
    /// </summary>
    public string OwnerName { get; set; }

    #endregion Properties
  }
}