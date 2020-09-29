using handshake.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace handshake.Contexts
{
  public class PersonInfoContext : DbContext
  {
    private readonly DbConnection connection;

    public PersonInfoContext(DbConnection connection)
    {
      this.connection = connection;
    }

    public DbSet<People> People { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(this.connection);
    }
  }
}
