using System;
using Microsoft.AspNetCore.Identity;

namespace cis2055_nemesys.Models
{
    public class User: IdentityUser
    {
        [PersonalData]

        public string FullName { get; set; }
    }
}
