using System;

namespace handshake.PostData
{
  /// <summary>
  /// The <see cref="GroupPostResultData"/> class is the response to creating a group.
  /// </summary>
  public class GroupPostResultData
  {
    #region Properties

    /// <summary>
    /// The id of the created group.
    /// </summary>
    public Guid Id { get; internal set; }

    #endregion Properties
  }
}