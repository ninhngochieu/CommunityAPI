using System;
using System.Collections.Generic;
using BackendAPI.Extentions;
using Microsoft.AspNetCore.Identity;

namespace BackendAPI.Models
{
    public class AppUser: IdentityUser<Guid>
    {
        public DateTime DateOfBirth { get; set; }
        public string KnownAs { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Introduction { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public string Gender { get; set; }
        public IList<Photo> Photos { get; set; }

        public IList<UserLike> LikeByUsers { get; set; }

        public IList<UserLike> LikedUsers { get; set; }
        public IList<Message> MessagesSent { get; set; }
        public IList<Message> MessageReceived { get; set; }
        public IList<AppUserRole> UserRoles { get; set; }
        public int GetAge()
        {
            return DateOfBirth.CalculateAge();
        }
    }
}