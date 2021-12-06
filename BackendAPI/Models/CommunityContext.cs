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

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }
    }
}
