using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace handshake.Entities
{
  public class Post
  {
    [Key]
    public Guid Id { get; set; }

    public Guid Author { get; set; }

    [MaxLength(1000)]
    public string Content { get; set; }

    public DateTime Creationdate { get; set; }

    public decimal Longitude { get; set; }

    public decimal Latitude { get; set; }
  }
}
