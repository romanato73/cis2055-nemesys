using cis2055_nemesys.Models;
using cis2055_nemesys.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace cis2055_nemesys.ViewModels
{
	public class AssignRoleViewModel
	{
        public UserViewModel User { get; set; }

        public IdentityRole[] Roles { get; set; }

        public string? RoleId { get; set; }
    }
}

