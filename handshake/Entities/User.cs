using System;
using System.ComponentModel.DataAnnotations;

namespace handshake.Entities
{
  public class User
  {
    [Key]
    public Guid Id { get; set; }

    [MaxLength(200)]
    public string Nickname { get; set; }

    [MaxLength(500)]
    public string Description { get; set; }
  }
}
