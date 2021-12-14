using System;
using Microsoft.AspNetCore.Identity;

namespace BackendAPI.Models
{
    public class AppUserRole: IdentityUserRole<Guid>
    {
        public AppUser User { get; set; }
        public AppRole Role { get; set; }
    }
}