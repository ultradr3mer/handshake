using System;

namespace handshake.PostData
{
  /// <summary>
  /// The insert post data is class for inserting new posts.
  /// </summary>
  public class PostPostData
  {
    /// <summary>
    /// The author who made the post.
    /// </summary>
    public Guid Author { get; set; }

    /// <summary>
    /// The contetn of the post.
    /// </summary>
    public string Content { get; set; }

    /// <summary>
    /// The post location longitude;
    /// </summary>
    public decimal Longitude { get; set; }

    /// <summary>
    /// The post location latitude;
    /// </summary>
    public decimal Latitude { get; set; }
  }
}
