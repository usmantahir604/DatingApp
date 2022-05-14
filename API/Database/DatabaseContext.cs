using API.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;

namespace API.Database
{
    public class DatabaseContext : IdentityDbContext<ApplicationUser>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options)
        {
        }
        public DbSet<AppUser> AppUsers { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }
        public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
                .HasKey(k => new { k.SourceUserId, k.LikedUserId });

            builder.Entity<UserLike>()
                   .HasOne(s => s.SourceUser)
                   .WithMany(l => l.LikedUsers)
                   .HasForeignKey(s => s.SourceUserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<UserLike>()
                   .HasOne(s => s.LikedUser)
                   .WithMany(l => l.LikedByUsers)
                   .HasForeignKey(s => s.LikedUserId)
                   .OnDelete(DeleteBehavior.NoAction);

            builder.Entity<Message>()
                   .HasOne(s => s.Recipient)
                   .WithMany(l => l.MessagesRecieved)
                   .OnDelete(DeleteBehavior.Restrict);

            builder.Entity<Message>()
                   .HasOne(s => s.Seder)
                   .WithMany(l => l.MessagesSent)
                   .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
