using HackFestHealthCare.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;

namespace HackFestHealthCare.Context
{
    public class HealthCareContext : DbContext
    {
        public HealthCareContext(DbContextOptions options) : base(options) { }

        public DbSet<UserProfile> UserProfile { get; set; }
        public DbSet<Role> Role { get; set; }
        public DbSet<UserRole> UserRole { get; set; }
        public DbSet<TokenModel> Tokens { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Role>().HasData(
                    new { RoleId = "2fc55484-d762-4eeb-b5db-ad531a4a0e04", RoleName = "admin", CreatedBy = Guid.Parse("F8E35EBC-6B86-4665-BF97-FE2E4FB74704") },
                    new { RoleId = "68e1b98f-7479-4f9b-aeed-8f10b8710f00", RoleName = "user", CreatedBy = Guid.Parse("F8E35EBC-6B86-4665-BF97-FE2E4FB74704") }
                );
            var propQuery = from entityType in modelBuilder.Model.GetEntityTypes()
                            from entityProperty in entityType.GetProperties()
                            where entityProperty.ClrType == typeof(decimal)
                            select entityProperty;

            foreach (var property in propQuery)
            {
                property.SetColumnType("decimal(18, 6)");
            }
        }
    }
}
