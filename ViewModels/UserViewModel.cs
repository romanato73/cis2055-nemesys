using Microsoft.AspNetCore.Identity;

namespace cis2055_nemesys.ViewModels
{
	public class UserViewModel
	{
        public string Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public IEnumerable<string> Roles { get; set; }
    }
}

