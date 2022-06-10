using cis2055_nemesys.Models;
using cis2055_nemesys.Models.Enums;
using Microsoft.AspNetCore.Identity;

namespace cis2055_nemesys.ViewModels
{
	public class UserListViewModel
	{
        public int TotalEntries { get; set; }

        public UserViewModel[] Users { get; set; }
    }
}

