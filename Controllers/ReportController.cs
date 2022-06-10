using Microsoft.AspNetCore.Mvc;
using cis2055_nemesys.Models;
using Microsoft.AspNetCore.Authorization;
using cis2055_nemesys.ViewModels;
using cis2055_nemesys.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace cis2055_nemesys.Controllers;

public class ReportController : Controller
{
    private readonly INemesysRepository _nemesysRepository;
    private readonly UserManager<User> _userManager;
    private readonly ILogger<ReportController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment;

    public ReportController(
            INemesysRepository nemesysRepository,
            UserManager<User> userManager,
            ILogger<ReportController> logger,
            IWebHostEnvironment webHostEnvironment)
    {
        _nemesysRepository = nemesysRepository;
        _userManager = userManager;
        _webHostEnvironment = webHostEnvironment;
        _logger = logger;
    }

    /// <summary>
    /// Show the form for creating reports.
    /// </summary>
    [HttpGet]
    [Authorize(Roles = "Reporter")]
    public IActionResult Create()
    {
        try
        {
            var viewModel = new ReportCreateViewModel();

            return View(viewModel);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message, e.Data);
            return View("Error");
        }
    }

    /// <summary>
    /// Store new report into database.
    /// </summary>
    /// <param name="newReport">
    /// Holds the form data of new Report.
    /// </param>
    [HttpPost]
    [Authorize(Roles = "Reporter")]
    public IActionResult Create([Bind("Location, Date, Time, Type, Description, Image")] ReportCreateViewModel newReport)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Set fileName for image
                string? path = null;

                // Check if has image
                if (newReport.Image != null)
                {
                    path = UploadImage(newReport.Image);
                }

                // Convert Date and Time into one object
                DateTime dateTime = Convert.ToDateTime(
                    newReport.Date + " " + newReport.Time
                );

                Report report = new()
                {
                    UserId = _userManager.GetUserId(User),
                    Location = newReport.Location,
                    Spotted = dateTime,
                    Type = newReport.Type,
                    Description = newReport.Description,
                    Image = path,
                    CreatedDate = DateTime.UtcNow,
                    UpdatedDate = DateTime.UtcNow,
                };

                _nemesysRepository.CreateReport(report);

                TempData["success"] = "Report has been created.";

                return RedirectToAction(
                    actionName: "MyReports",
                    controllerName: "Dashboard"
                );
            }
            else
            {
                return View(newReport);
            }
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e.Message);
            return View(newReport);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message, e.Data);
            return View("Error");
        }
    }

    /// <summary>
    /// Show the report by ID
    /// </summary>
    /// <param name="id">ID of the Report</param>
    [Authorize]
    public IActionResult Show(int id)
    {
        try
        {
            var report = _nemesysRepository.GetReportById(id);

            if (report == null)
            {
                return NotFound();
            }

            // Check if user upvoted this report
            bool upvoted = _nemesysRepository.UserHasUpvotedReport(report, _userManager.GetUserId(User));

            // Get the reporter
            var ReporterUser = _userManager.FindByIdAsync(report.UserId).Result;

            var viewModel = new ReportViewModel()
            {
                Id = report.Id,
                Type = report.Type,
                Location = report.Location,
                Spotted = report.Spotted,
                Description = report.Description,
                Status = report.Status,
                User = new UserViewModel()
                {
                    Id = report.UserId,
                    FullName = ReporterUser.FullName,
                    Email = ReporterUser.Email,
                    PhoneNumber = ReporterUser.PhoneNumber,
                },
                Image = report.Image,
                UserHasUpvoted = upvoted,
                UpvotesCount = _nemesysRepository.CountUpvotesForReport(report.Id),
                Investigation = report.Investigation == null ? null : new InvestigationViewModel()
                {
                    Id = report.Investigation.Id,
                    Description = report.Investigation.Description,
                    ActionDate = report.Investigation.ActionDate,
                    CreatedDate = report.Investigation.CreatedDate,
                    UpdatedDate = report.Investigation.UpdatedDate,
                    User = new UserViewModel()
                    {
                        Id = report.Investigation.User.Id,
                        FullName = report.Investigation.User.FullName,
                        Email = report.Investigation.User.Email,
                        PhoneNumber = report.Investigation.User.PhoneNumber,
                    }
                },
                CreatedDate = report.CreatedDate,
                UpdatedDate = report.UpdatedDate,
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
    /// Edit report form
    /// </summary>
    /// <param name="id">ID of the report we want to edit</param>
    [HttpGet]
    [Authorize(Roles = "Reporter")]
    public IActionResult Edit(int id)
    {
        try
        {
            var report = _nemesysRepository.GetReportById(id);

            if (report == null)
            {
                return NotFound();
            }

            if (report.UserId != _userManager.GetUserId(User))
            {
                return Forbid();
            }

            var viewModel = new ReportEditViewModel()
            {
                Id = report.Id,
                Type = report.Type,
                Location = report.Location,
                Date = report.Spotted.ToString("yyyy-MM-dd"),
                Time = report.Spotted.ToString("HH:mm"),
                Description = report.Description,
                ImageUrl = report.Image,
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
    /// Update the report to the database
    /// </summary>
    /// <param name="id">ID of the report to be updated</param>
    /// <param name="updatedReport">Fields being updated</param>
    [Authorize(Roles = "Reporter")]
    [HttpPost]
    public IActionResult Edit(
        int id,
        [Bind("Location, Date, Time, Type, Description, Image")] ReportEditViewModel updatedReport
    )
    {
        try
        {
            var modelToUpdate = _nemesysRepository.GetReportById(id);

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
                string? path = null;

                // Check if has image
                if (updatedReport.Image != null)
                {
                    path = UploadImage(updatedReport.Image);
                }

                DateTime dateTime = Convert.ToDateTime(
                    updatedReport.Date + " " + updatedReport.Time
                );

                // Update report
                modelToUpdate.UserId = _userManager.GetUserId(User);
                modelToUpdate.Location = updatedReport.Location;
                modelToUpdate.Spotted = dateTime;
                modelToUpdate.Type = updatedReport.Type;
                modelToUpdate.Description = updatedReport.Description;
                modelToUpdate.Image = path ?? modelToUpdate.Image;
                modelToUpdate.UpdatedDate = DateTime.UtcNow;

                _nemesysRepository.UpdateReport(modelToUpdate);

                TempData["success"] = "Report has been updated.";

                // Return to dashboard
                return RedirectToAction(
                    actionName: "Show",
                    controllerName: "Report",
                    new { id = updatedReport.Id }
                );
            }
            else
            {
                return View(updatedReport);
            }
        }
        catch (DbUpdateException e)
        {
            _logger.LogError(e.Message);
            return View(updatedReport);
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message, e.Data);
            return View("Error");
        }
    }

    /// <summary>
    /// <para>Delete the report</para>
    /// <para>Check if report is not under investigation</para>
    /// </summary>
    /// <param name="id">ID of the report to be deleted</param>
    [Authorize(Roles = "Reporter")]
    [HttpPost]
    public IActionResult Delete(int id)
    {
        var report = _nemesysRepository.GetReportById(id);

        if (report == null)
        {
            return NotFound();
        }

        if (report.UserId != _userManager.GetUserId(User))
        {
            return Forbid();
        }

        // Check if has investigation
        if (report.Investigation != null)
        {
            return Forbid();
        }

        _nemesysRepository.DeleteReport(report);

        TempData["success"] = "Report has been deleted.";

        return RedirectToAction(
            actionName: "MyReports",
            controllerName: "Dashboard"
        );
    }

    /// <summary>
    /// Upload image
    /// </summary>
    /// <param name="file">File to be uploaded</param>
    /// <returns>Path of uploaded file</returns>
    private string UploadImage(IFormFile file)
    {
        string uploadsFolder = Path.Combine(_webHostEnvironment.WebRootPath, "images/reports");

        string extension = Path.GetExtension(file.FileName);

        string fileName = Guid.NewGuid().ToString() + extension;

        string path = Path.Combine(uploadsFolder, fileName);

        using (var bits = new FileStream(path, FileMode.Create))
        {
            file.CopyTo(bits);
        }

        return path;
    }
}

