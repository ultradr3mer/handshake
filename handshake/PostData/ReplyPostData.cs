using System;
using System.ComponentModel.DataAnnotations;

namespace handshake.PostData
{
  /// <summary>
  /// A post.
  /// </summary>
  public class ReplyPostData
  {
    #region Properties

    /// <summary>
    /// The content.
    /// </summary>
    [MaxLength(1000)]
    public string Content { get; set; }

    /// <summary>
    /// The post replyed on.
    /// </summary>
    public Guid Post { get; set; }

    #endregion Properties
  }
}