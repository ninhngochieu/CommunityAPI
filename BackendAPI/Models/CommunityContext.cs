using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Mail;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace BackendAPI.Models
{
    public class CommunityContext: IdentityDbContext<AppUser, AppRole, Guid,
        IdentityUserClaim<Guid>, AppUserRole, IdentityUserLogin<Guid>,IdentityRoleClaim<Guid>,IdentityUserToken<Guid>>
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
        public virtual DbSet<Message> Messages { get; set; }
        public virtual DbSet<Group>Groups { get; set; }
        public virtual DbSet<Connection> Connections { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);// Sẽ xảy ra lỗi migration nếu ko có
            
            modelBuilder.Entity<AppUser>()
                .HasMany(u => u.UserRoles)
                .WithOne(u => u.User)
                .HasForeignKey(u => u.UserId)
                .IsRequired();
            
            modelBuilder.Entity<AppRole>()
                .HasMany(u => u.UserRoles)
                .WithOne(u => u.Role)
                .HasForeignKey(u => u.RoleId)
                .IsRequired();
            
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

            modelBuilder.Entity<Message>()
                .HasOne(u=>u.RecipientUser)
                .WithMany(m=>m.MessageReceived)
                .OnDelete(DeleteBehavior.Restrict);
            
            modelBuilder.Entity<Message>()
                .HasOne(u=>u.SenderUser)
                .WithMany(m=>m.MessagesSent)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
