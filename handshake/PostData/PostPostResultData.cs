using System;

namespace handshake.PostData
{
  /// <summary>
  /// The <see cref="PostPostData"/> contains information about the newly created post.
  /// </summary>
  public class PostPostResultData
  {
    #region Properties

    /// <summary>
    /// The id of the post.
    /// </summary>
    public Guid Id { get; set; }

    #endregion Properties
  }
}