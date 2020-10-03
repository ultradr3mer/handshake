using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace handshake.Entities
{
  /// <summary>
  /// A post.
  /// </summary>
  public class Post
  {
    /// <summary>
    /// The id.
    /// </summary>
    [Key]
    public Guid Id { get; set; }

    /// <summary>
    /// The author.
    /// </summary>
    public Guid Author { get; set; }

    /// <summary>
    /// The content.
    /// </summary>
    [MaxLength(1000)]
    public string Content { get; set; }

    /// <summary>
    /// The creation date.
    /// </summary>
    public DateTime Creationdate { get; set; }

    /// <summary>
    /// The location longitude.
    /// </summary>
    public decimal Longitude { get; set; }

    /// <summary>
    /// The location lattitude.
    /// </summary>
    public decimal Latitude { get; set; }
  }
}
