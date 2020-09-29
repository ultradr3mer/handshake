﻿using handshake.Entities;
using Microsoft.EntityFrameworkCore;
using System.Data.Common;

namespace handshake.Contexts
{
  internal class DatabaseContext : DbContext
  {
    private readonly DbConnection connection;

    public DatabaseContext(DbConnection connection)
    {
      this.connection = connection;
    }

    public DbSet<User> User { get; set; }
    public DbSet<Post> Post { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(this.connection);
    }
  }
}