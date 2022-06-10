using Microsoft.AspNetCore.Mvc;
using cis2055_nemesys.Models;
using Microsoft.AspNetCore.Authorization;
using cis2055_nemesys.ViewModels;
using cis2055_nemesys.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace cis2055_nemesys.Controllers;

public class InvestigationController : Controller
{
    private readonly INemesysRepository _nemesysRepository;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<InvestigationController> _logger;

    public InvestigationController(
            INemesysRepository nemesysRepository,
            UserManager<User> userManager,
            ILogger<InvestigationController> logger)
    {
        _nemesysRepository = nemesysRepository;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// List of all reports that user is investigating
    /// </summary>
    /// <param name="showAll">
    /// Determines if we want to show also closed reports
    /// </param>
    [Authorize(Roles = "Investigator")]
    public IActionResult Index(string? showAll)
    {
        try
        {
            var userId = _userManager.GetUserId(User);

            var viewModel = new ReportListViewModel()
            {
                TotalEntries = _nemesysRepository
                .GetAllMyInvestigations(userId, showAll != null)
                .Count(),
                Reports = _nemesysRepository
                .GetAllMyInvestigations(userId, showAll != null)
                .Select(r => new ReportViewModel
                {
                    Id = r.Id,
                    Type = r.Type,
                    Location = r.Location,
                    Description = r.Description,
                    Status = r.Status,
                    User = new UserViewModel
                    {
                        Id = r.UserId,
                        FullName = r.User.FullName,
                    },
                    CreatedDate = r.CreatedDate,
                    UpdatedDate = r.UpdatedDate,
                })
            };

            return View(viewModel);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message, e.Data);
            throw;
        }
    }

    /// <summary>
    /// Create Investigation form.
    /// </summary>
    /// <param name="reportId">
    /// Report ID we want to create a investigation.
    /// </param>
    [HttpGet]
    [Authorize(Roles = "Investigator")]
    [Route("/Investigation/Create/{reportId}")]
    public IActionResult Create(int reportId)
    {
        try
        {
            var report = _nemesysRepository.GetReportById(reportId);

            if (report == null || report.Investigation != null)
            {
                return Forbid();
            }

            var viewModel = new InvestigationFormViewModel();

            return View(viewModel);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message, e.Data);
            return View("Error");
        }
    }

    /// <summary>
    /// Store new investigation into database.
    /// </summary>
    /// <param name="reportId">
    /// Report ID we want to create a investigation.
    /// </param>
    /// <param name="newInvestigation">
    /// Holds the form parameters.
    /// </param>
    [HttpPost]
    [Authorize(Roles = "Investigator")]
    [Route("/Investigation/Create/{reportId}")]
    public IActionResult Create(
        int reportId,
        [Bind("ReportId, Description, ActionDate, ActionTime")] InvestigationFormViewModel newInvestigation
    )
    {
        try
        {
            if (ModelState.IsValid)
            {
                DateTime dateTime = Convert.ToDateTime(
                    newInvestigation.ActionDate + " " + newInvestigation.ActionTime
                );

                Investigation investigation = new()
                {
                    UserId = _userManager.GetUserId(User),
                    ReportId = newInvestigation.ReportId,
                    Description = newInvestigation.Description,
                    ActionDate = dateTime,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                };

                _nemesysRepository.CreateInvestigation(investigation);

                _nemesysRepository.UpdateReportStatus(reportId, Models.Enums.ReportStatus.BeingInvestigated);

                TempData["success"] = "Investigation has been created.";

                return RedirectToAction(
                    actionName: "Show",
                    controllerName: "Report",
                    new { id = reportId }
                );
            }
            else
            {
                return View(newInvestigation);
            }
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e.Message);
            return View(newInvestigation);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message, e.Data);
            return View("Error");
        }
    }

    /// <summary>
    /// Edits the investigation
    /// </summary>
    /// <param name="id">
    /// ID of Investigation we want to edit
    /// </param>
    [HttpGet]
    [Authorize(Roles = "Investigator")]
    [Route("/Investigation/Edit/{id}")]
    public IActionResult Edit(int id)
    {
        try
        {
            var investigation = _nemesysRepository.GetInvestigationById(id);

            if (investigation == null)
            {
                return NotFound();
            }

            if (investigation.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            var viewModel = new InvestigationFormViewModel()
            {
                Id = investigation.Id,
                Description = investigation.Description,
                ActionDate = investigation.ActionDate.ToString("yyyy-MM-dd"),
                ActionTime = investigation.ActionDate.ToString("HH:mm"),
                ReportId = investigation.ReportId,
                ReportStatus = investigation.Report.Status,
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
    /// Update investigation into database
    /// </summary>
    /// <param name="id">ID of investigation to be updated</param>
    /// <param name="updatedInvestigation">
    /// Updated investigation form data
    /// </param>
    [HttpPost]
    [Authorize(Roles = "Investigator")]
    [Route("/Investigation/Edit/{id}")]
    public IActionResult Edit(
        int id,
        [Bind("Description, ActionDate, ActionTime, ReportId, ReportStatus")] InvestigationFormViewModel updatedInvestigation
    ) {
        try
        {
            var modelToUpdate = _nemesysRepository.GetInvestigationById(id);

            if (modelToUpdate == null)
            {
                return NotFound();
            }

            if (modelToUpdate.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            if (ModelState.IsValid)
            {
                DateTime dateTime = Convert.ToDateTime(
                    updatedInvestigation.ActionDate + " " + updatedInvestigation.ActionTime
                );

                // Update investigation
                modelToUpdate.UserId = _userManager.GetUserId(User);
                modelToUpdate.Description = updatedInvestigation.Description;
                modelToUpdate.ActionDate = dateTime;
                modelToUpdate.UpdatedDate = DateTime.UtcNow;

                _nemesysRepository.UpdateInvestigation(modelToUpdate);

                _nemesysRepository.UpdateReportStatus(updatedInvestigation.ReportId, updatedInvestigation.ReportStatus);

                TempData["success"] = "Investigation has been updated.";

                return RedirectToAction(
                    actionName: "Show",
                    controllerName: "Report",
                    new { id = updatedInvestigation.ReportId }
                );
            }
            else
            {
                return View(updatedInvestigation);
            }
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e.Message);
            return View(updatedInvestigation);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message, e.Data);
            return View("Error");
        }
    }
}

