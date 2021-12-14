using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Identity;

namespace BackendAPI.Models
{
    public class AppRole: IdentityRole<Guid>
    {
        public IList<AppUserRole> UserRoles { get; set; }
    }
}