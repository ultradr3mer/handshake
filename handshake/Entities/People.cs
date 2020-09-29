using System.ComponentModel.DataAnnotations;

namespace handshake.Entities
{
  public class People
  {
    [Key]
    public int Lfdnr { get; set; }

    [MaxLength(100)]
    public string Name { get; set; }

    [MaxLength(100)]
    public string Name2 { get; set; }

    public int Age { get; set; }
  }
}
