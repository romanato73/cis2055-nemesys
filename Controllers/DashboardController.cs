using Microsoft.AspNetCore.Mvc;
using cis2055_nemesys.Models;
using Microsoft.AspNetCore.Authorization;
using cis2055_nemesys.ViewModels;
using cis2055_nemesys.Models.Interfaces;
using Microsoft.AspNetCore.Identity;

namespace cis2055_nemesys.Controllers;

public class DashboardController : Controller
{
    private readonly INemesysRepository _nemesysRepository;
    private readonly UserManager<User> _userManager;
    private readonly ILogger _logger;

    public DashboardController(
        INemesysRepository nemesysRepository,
        UserManager<User> userManager,
        ILogger<DashboardController> logger
    )
    {
        _nemesysRepository = nemesysRepository;
        _userManager = userManager;
        _logger = logger;
    }

    /// <summary>
    /// <para>Show the list of reports.</para>
    /// <para>
    /// If user is not confirmed (Guest) he is redirected to Unconfirmed method
    /// </para>
    /// </summary>
    /// <param name="showAll">
    /// Determines if we want to see also closed reports
    /// </param>
    [Authorize]
    public IActionResult Index(string? showAll)
    {
        if (User.IsInRole("Guest"))
        {
            return RedirectToAction("Unconfirmed");
        }

        try
        {
            var viewModel = new ReportListViewModel()
            {
                TotalEntries = _nemesysRepository.GetAllReports(showAll != null).Count(),
                Reports = _nemesysRepository
                .GetAllReports(showAll != null)
                .Select(r => new ReportViewModel
                {
                    Id = r.Id,
                    Type = r.Type,
                    Location = r.Location,
                    Spotted = r.Spotted,
                    Description = r.Description,
                    Status = r.Status,
                    User = new UserViewModel
                    {
                        Id = r.UserId,
                        FullName = r.User.FullName,
                        Email = r.User.Email,
                        PhoneNumber = r.User.PhoneNumber,
                    },
                    Image = r.Image,
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
    /// Show the list of my reports.
    /// </summary>
    /// <param name="showAll">
    /// Determines if we want to see also closed reports
    /// </param>
    [Authorize(Roles = "Reporter")]
    public IActionResult MyReports(string? showAll)
    {
        try
        {
            var userId = _userManager.GetUserId(User);

            var viewModel = new ReportListViewModel()
            {
                TotalEntries = _nemesysRepository.GetAllMyReports(userId, showAll != null).Count(),
                Reports = _nemesysRepository
                    .GetAllMyReports(userId, showAll != null)
                    .Select(r => new ReportViewModel
                    {
                        Id = r.Id,
                        Type = r.Type,
                        Location = r.Location,
                        Spotted = r.Spotted,
                        Description = r.Description,
                        Status = r.Status,
                        User = new UserViewModel
                        {
                            Id = r.UserId,
                            FullName = r.User.FullName,
                            Email = r.User.Email,
                            PhoneNumber = r.User.PhoneNumber,
                        },
                        Image = r.Image,
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
    /// Show the page for unconfirmed (Guests) users
    /// </summary>
    [Authorize(Roles = "Guest")]
    public IActionResult Unconfirmed()
    {
        return View();
    }

    /// <summary>
    /// Show the hall of fame page
    /// </summary>
    [Authorize(Roles = "Admin,Reporter,Investigator")]
    [Route("/Dashboard/Hall-Of-Fame")]
    public IActionResult HallOfFame()
    {
        var dict = new Dictionary<string, ReportStatViewModel>();

        var repo = _nemesysRepository.GetReportsForStatistics();

        foreach (var record in repo)
        {
            var item = dict.GetValueOrDefault(record.UserId);

            if (item == null)
            {
                dict.Add(record.UserId, new ReportStatViewModel
                {
                    TotalReports = 1,
                    FullName = record.User.FullName,
                });
            } else
            {
                item.TotalReports++;
            }
        }

        var viewModel = new ReportStatsViewModel
        {
            Users = dict.OrderByDescending(item => item.Value.TotalReports).Select(item => new ReportStatViewModel
            {
                FullName = item.Value.FullName,
                UserId = item.Value.UserId,
                TotalReports = item.Value.TotalReports,
            })
        };

        return View(viewModel);
    }
}

