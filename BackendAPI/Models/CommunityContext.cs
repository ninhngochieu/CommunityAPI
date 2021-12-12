using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Models
{
    public class CommunityContext: DbContext
    {
        public CommunityContext()
        {
        }

        public CommunityContext([NotNullAttribute] DbContextOptions options) : base(options)
        {
        }

        public virtual DbSet<AppUser> AppUsers{ get; set; }
        public virtual DbSet<Photo>  Photos { get; set; }
        public virtual DbSet<UserLike> Likes { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);// Sẽ xảy ra lỗi migration nếu ko có
            modelBuilder.Entity<UserLike>()
                .HasKey(k => new {k.LikedUserId, k.SourceUserId});

            modelBuilder.Entity<UserLike>() // Like những người nào
                .HasOne(s => s.SourceUser)
                .WithMany(l => l.LikedUsers)
                .HasForeignKey(s => s.SourceUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
            modelBuilder.Entity<UserLike>()// Được like bởi người nào
                .HasOne(l=>l.LikedUser)
                .WithMany(s => s.LikeByUsers)
                .HasForeignKey(l => l.LikedUserId)
                .OnDelete(DeleteBehavior.Cascade);
            
        }
    }
}
