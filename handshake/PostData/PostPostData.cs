using System;
using System.ComponentModel.DataAnnotations;

namespace handshake.PostData
{
  /// <summary>
  /// The insert post data is class for inserting new posts.
  /// </summary>
  public class PostPostData
  {
    /// <summary>
    /// The contetn of the post.
    /// </summary>
    [MaxLength(1000)]
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
