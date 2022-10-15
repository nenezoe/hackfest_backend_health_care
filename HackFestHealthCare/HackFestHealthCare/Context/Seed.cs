using HackFestHealthCare.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Linq;

namespace HackFestHealthCare.Context
{
    public class Seed
    {
        private readonly HealthCareContext _context;
        public Seed(HealthCareContext context)
        {
            _context = context;
        }

        public void SeedUsers()
        {
            if (_context.UserProfile.Any())
            {
                return;
            }
            var hasher = new PasswordHasher<UserProfile>();
            var user = new UserProfile()
            {
                Id = Guid.Parse("4F909566-E5E5-4796-A418-25B81D611763"),
                Email = "jpapa555@yahoo.com",
                FirstName = "Joe",
                LastName = "Papa",
                PhoneNumber = "07032369246",
                IsAccountActivated = true,
                Address = "Surulere, Lagos",
                CreatedBy = Guid.Parse("4F909566-E5E5-4796-A418-25B81D611763"),
                PasswordHash = hasher.HashPassword(null, "P@55word")
            };
            _context.Add(user);

            var userRole = new UserRole()
            {
                Id = Guid.NewGuid(),
                RoleId = "2fc55484-d762-4eeb-b5db-ad531a4a0e04",
                UserId = Guid.Parse("4F909566-E5E5-4796-A418-25B81D611763"),
                CreatedBy = Guid.Parse("4F909566-E5E5-4796-A418-25B81D611763"),
                CreatedAt = DateTime.Now,
                IsActive = true,
                IsDeleted = false
            };
            _context.Add(userRole);
            _context.SaveChanges();
        }
    }
}
