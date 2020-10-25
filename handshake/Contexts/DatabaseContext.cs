using handshake.Entities;
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

    public DbSet<UserEntity> ShakeUser { get; set; }
    public DbSet<PostEntity> Post { get; set; }
    public DbSet<ReplyEntity> Reply { get; set; }
    public DbSet<InviteEntity> Invite { get; set; }
    public DbSet<FileAccessTokenEntity> FileAccessToken { get; set; }
    public DbSet<GroupEntity> ShakeGroup { get; set; }
    public DbSet<UserGroupEntity> UserGroup { get; set; }
    public DbSet<PostGroupEntity> PostGroup { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
      optionsBuilder.UseSqlServer(this.connection);
    }
  }
}
