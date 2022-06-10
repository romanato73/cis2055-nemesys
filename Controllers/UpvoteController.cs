using cis2055_nemesys.Models;
using cis2055_nemesys.Models.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace cis2055_nemesys.Controllers
{
    public class UpvoteController : Controller
    {
        private readonly INemesysRepository _nemesysRepository;
        private readonly UserManager<User> _userManager;
        private readonly ILogger<UpvoteController> _logger;

        public UpvoteController(
            INemesysRepository nemesysRepository,
            UserManager<User> userManager,
            ILogger<UpvoteController> logger
        )
        {
            _nemesysRepository = nemesysRepository;
            _userManager = userManager;
            _logger = logger;
        }

        /// <summary>
        /// Store new upvote into database
        /// </summary>
        /// <param name="reportId">
        /// ID of report that will have the upvote
        /// </param>
        [Authorize(Roles = "Reporter")]
        [HttpPost]
        [Route("/Report/Upvote/{reportId}/Create")]
        public IActionResult Create(int reportId)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var report = _nemesysRepository.GetReportById(reportId);

                    if (report == null)
                    {
                        return RedirectToAction(
                            actionName: "Show",
                            controllerName: "Report",
                            new { id = reportId }
                        );
                    }

                    if (report.UserId == _userManager.GetUserId(User))
                    {
                        return Forbid();
                    }

                    if (!User.IsInRole("Reporter"))
                    {
                        return Forbid();
                    }

                    Upvote upvote = new()
                    {
                        UserId = _userManager.GetUserId(User),
                        ReportId = reportId,
                    };

                    _nemesysRepository.CreateUpvote(upvote);

                    TempData["success"] = "Report has been upvoted.";

                    return RedirectToAction(
                        actionName: "Show",
                        controllerName: "Report",
                        new { id = reportId }
                    );
                }
                else
                {
                    return RedirectToAction(
                        actionName: "Show",
                        controllerName: "Report",
                        new { id = reportId }
                    );
                }
            }
            catch (DbUpdateException e)
            {
                _logger.LogError(e.Message);
                throw;
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message, e.Data);
                return View("Error");
            }
        }

        /// <summary>
        /// Deletes the upvote for report
        /// </summary>
        /// <param name="reportId">
        /// ID of the report to which will be deleted upvote
        /// </param>
        [Authorize(Roles = "Reporter")]
        [HttpPost]
        [Route("/Report/Upvote/{reportId}/Delete")]
        public IActionResult Delete(int reportId)
        {
            var report = _nemesysRepository.GetReportById(reportId);

            if (report == null)
            {
                return NotFound();
            }

            if (report.UserId == _userManager.GetUserId(User))
            {
                return Forbid();
            }

            _nemesysRepository.DeleteUpvote(reportId, _userManager.GetUserId(User));

            TempData["success"] = "Report has been downvoted.";

            return RedirectToAction(
                actionName: "Show",
                controllerName: "Report",
                new { id = reportId }
            );
        }
    }
}

