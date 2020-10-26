using handshake.Entities;
using handshake.Extensions;
using System;

namespace handshake.Data
{
  /// <summary>
  /// The <see cref="AssociatedGroupData"/> is an associated group.
  /// </summary>
  public class AssociatedGroupData
  {
    #region Constructors

    /// <summary>
    /// Creates a new instance of the <see cref="AssociatedGroupData"/> based on an <see cref="GroupEntity"/>.
    /// </summary>
    /// <param name="entity">The <see cref="GroupEntity"/> to copy properties from.</param>
    public AssociatedGroupData(GroupEntity entity)
    {
      this.CopyPropertiesFrom(entity);
    }

    #endregion Constructors

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