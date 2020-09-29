using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace handshake.SetDaten
{
  public class InsertPostDaten
  {
    public Guid Author { get; set; }

    public string Content { get; set; }

    public decimal Longitude { get; set; }

    public decimal Latitude { get; set; }
  }
}
