using cis2055_nemesys.Models;
using cis2055_nemesys.Models.Interfaces;
using cis2055_nemesys.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace cis2055_nemesys.Controllers
{
    [Route("[controller]")]
    public class AdminController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly INemesysRepository _nemesysRepository;
        private readonly ILogger<ReportController> _logger;

        public AdminController(
            UserManager<User> userManager,
            RoleManager<IdentityRole> roleManager,
            INemesysRepository nemesysRepository,
            ILogger<ReportController> logger
        )
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _nemesysRepository = nemesysRepository;
            _logger = logger;
        }

        /// <summary>
        /// Show the list of Users
        /// </summary>
        [Authorize(Roles = "Admin")]
        public IActionResult Index()
        {
            try
            {
                var users = _userManager.Users;

                var viewModel = new UserListViewModel()
                {
                    TotalEntries = users.Count(),
                    Users = users.Select(user => new UserViewModel
                    {
                        Id = user.Id,
                        FullName = user.FullName,
                        Email = user.Email,
                        PhoneNumber = user.PhoneNumber,
                        Roles = _userManager.GetRolesAsync(user).Result.ToArray(),
                    }).ToArray()
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.Data);
                return View("Error");
            }
        }

        /// <summary>
        /// Show the form for the assigning user
        /// </summary>
        /// <param name="userId">ID of User that we want to assign a role</param>
        [Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("assign/{userId}/role")]
        public IActionResult AssignRole(string userId)
        {
            try
            {
                var user = _userManager.FindByIdAsync(userId).Result;

                var viewModel = new AssignRoleViewModel()
                {
                    User = new UserViewModel()
                    {
                        Id = user.Id,
                        FullName = (user != null) ? user.FullName : "Anonymous",
                        Email = (user != null) ? user.Email : "Unknown",
                        PhoneNumber = (user != null) ? user.PhoneNumber : "Unknown",
                        Roles = _userManager.GetRolesAsync(user).Result.ToArray(),
                    },
                    Roles = _roleManager.Roles.ToArray(),
                };

                return View(viewModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.Data);
                return View("Error");
            }
        }

        /// <summary>
        /// Assign the role to the user
        /// </summary>
        /// <param name="userId">ID of User that we want to assign a role</param>
        /// <param name="assignRole">Gets the RoleId from AssignRoleViewModel</param>
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [Route("assign/{userId}/role")]
        public async Task<IActionResult> AssignRole(string userId, [Bind("RoleId")] AssignRoleViewModel assignRole)
        {
            try
            {
                var roleId = assignRole.RoleId;

                if (roleId == null)
                {
                    return RedirectToAction("AssignRole");
                }

                await _nemesysRepository.AssignRole(userId, roleId);

                TempData["success"] = "Role has been assigned.";

                return RedirectToAction("AssignRole");
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.Data);
                return View("Error");
            }
        }
    }
}

