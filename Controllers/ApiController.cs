using cis2055_nemesys.Models;
using cis2055_nemesys.Models.Interfaces;
using cis2055_nemesys.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace cis2055_nemesys.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class ApiController : Controller
    {
        private readonly INemesysRepository _nemesysRepository;
        private readonly UserManager<User> _userManager;

        public ApiController(
            INemesysRepository nemesysRepository,
            UserManager<User> userManager
        )
        {
            _nemesysRepository = nemesysRepository;
            _userManager = userManager;
        }

        /// <summary>
        /// Get the information about reporting and investigating
        /// for sidebar statistics.
        /// </summary>
        /// <returns>JSON Response</returns>
        [Authorize(Roles = "Reporter,Investigator")]
        [HttpGet]
        [Route("user/{id}/reports")]
        public IActionResult UserReports(string id)
        {
            var user = _userManager.FindByIdAsync(id).Result;

            if (user == null)
            {
                return Unauthorized();
            }

            var userRoles = _userManager.GetRolesAsync(user).Result;

            var role = userRoles.FirstOrDefault();

            if (role == null)
            {
                return Unauthorized();
            }

            IEnumerable<Report> repo;

            if (role == "Investigator")
            {
                repo = _nemesysRepository.GetAllMyInvestigations(id, true);
            }
            else
            {
                repo = _nemesysRepository.GetAllMyReports(id, true);
            }

            var data = new SidebarStatsViewModel
            {
                TotalReports = repo.Count(),
                Open = repo.Where(r => r.Status == Models.Enums.ReportStatus.Open).Count(),
                BeingInvestigated = repo.Where(r => r.Status == Models.Enums.ReportStatus.BeingInvestigated).Count(),
                NoActionRequired = repo.Where(r => r.Status == Models.Enums.ReportStatus.NoActionRequired).Count(),
                Closed = repo.Where(r => r.Status == Models.Enums.ReportStatus.Closed).Count(),
            };

            return Ok(data);
        }
    }
}

