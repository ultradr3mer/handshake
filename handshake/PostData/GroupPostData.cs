using System.ComponentModel.DataAnnotations;

namespace handshake.PostData
{
  /// <summary>
  /// A group.
  /// </summary>
  public class GroupPostData
  {
    #region Properties

    /// <summary>
    /// The description.
    /// </summary>
    [MaxLength(500)]
    public string Description { get; set; }

    /// <summary>
    /// The name of the group.
    /// </summary>
    [MaxLength(200)]
    public string Name { get; set; }

    #endregion Properties
  }
}