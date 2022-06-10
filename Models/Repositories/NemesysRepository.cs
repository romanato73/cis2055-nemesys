using cis2055_nemesys.Models.Enums;
using cis2055_nemesys.Models.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace cis2055_nemesys.Models.Repositories;

public class NemesysRepository : INemesysRepository
{
    private readonly ApplicationDbContext _appDb;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<IdentityRole> _roleManager;
    private readonly ILogger _logger;

    public NemesysRepository(
        ApplicationDbContext appDbContext,
        UserManager<User> userManager,
        RoleManager<IdentityRole> roleManager,
        ILogger<NemesysRepository> logger
    ) {
        _appDb = appDbContext;
        _userManager = userManager;
        _roleManager = roleManager;
        _logger = logger;
    }

    /**
     * +----------------------------------------------------------+
     * |                        REPORTS                           |
     * +----------------------------------------------------------+
     */

    /// <summary>
    /// Get all reports
    /// </summary>
    /// <param name="closed">Determines if show also closed reports</param>
    /// <returns>Array of reports</returns>
    public IEnumerable<Report> GetAllReports(bool closed = false)
    {
        try
        {
            var reports = _appDb.Reports.AsQueryable();

            if (!closed)
            {
                reports = reports.Where(r => r.Status != ReportStatus.Closed);
            }

            return reports
                .Include(r => r.User)
                .OrderByDescending(r => r.CreatedDate);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Get all reports for user
    /// </summary>
    /// <param name="userId">ID of user that owns the report</param>
    /// <param name="closed">Determines if show also closed reports</param>
    /// <returns>Array of reports</returns>
    public IEnumerable<Report> GetAllMyReports(string userId, bool closed = false)
    {
        try
        {
            var reports = _appDb.Reports.Where(r => r.UserId == userId);

            if (!closed) {
                reports = reports.Where(r => r.Status != ReportStatus.Closed);
            }

            return reports.Include(r => r.User).OrderByDescending(r => r.CreatedDate);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets report by id
    /// </summary>
    /// <param name="id">ID of report</param>
    /// <returns>Report or null if not found</returns>
    public Report? GetReportById(int id)
    {
        try
        {
            return _appDb.Reports
                .Include(r => r.User)
                .Include(r => r.Investigation)
                .Include(r => r.Investigation.User)
                .FirstOrDefault(r => r.Id == id);
        } catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Stores new report into database
    /// </summary>
    /// <param name="newReport"></param>
    public void CreateReport(Report newReport)
    {
        try
        {
            _appDb.Reports.Add(newReport);
            _appDb.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Updates report into database
    /// </summary>
    /// <param name="report"></param>
    public void UpdateReport(Report report)
    {
        try
        {
            var existingReport = _appDb.Reports.FirstOrDefault(r => r.Id == report.Id);

            if (existingReport != null)
            {
                existingReport.Type = report.Type;
                existingReport.Location = report.Location;
                existingReport.Spotted = report.Spotted;
                existingReport.Description = report.Description;
                existingReport.Image = report.Image;

                _appDb.Entry(existingReport).State = EntityState.Modified;
                _appDb.SaveChanges();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Updates report status
    /// </summary>
    /// <param name="id">ID of report to be updated</param>
    /// <param name="newStatus"></param>
    public void UpdateReportStatus(int id, ReportStatus newStatus)
    {
        try
        {
            var existingReport = _appDb.Reports.FirstOrDefault(r => r.Id == id);

            if (existingReport != null)
            {
                existingReport.Status = newStatus;

                _appDb.Entry(existingReport).State = EntityState.Modified;
                _appDb.SaveChanges();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Deletes report from database
    /// </summary>
    /// <param name="report"></param>
    public void DeleteReport(Report report)
    {
        try
        {
            _appDb.Reports.Remove(report);
            _appDb.Entry(report).State = EntityState.Deleted;
            _appDb.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /**
     * +----------------------------------------------------------+
     * |                     INVESTIGATIONS                       |
     * +----------------------------------------------------------+
     */

    /// <summary>
    /// Gets the Investigation by Report ID
    /// </summary>
    /// <param name="reportId">Report ID that has investigation</param>
    /// <returns>Investigation or null if not found</returns>
    public Investigation? GetInvestigationByReportId(int reportId)
    {
        try
        {
            return _appDb.Investigations
                .Include(i => i.User)
                .FirstOrDefault(i => i.ReportId == reportId);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Gets the Investigation by ID
    /// </summary>
    /// <param name="id">ID of investigation</param>
    /// <returns>Investigation or null if not found</returns>
    public Investigation? GetInvestigationById(int id)
    {
        try
        {
            return _appDb.Investigations
                .Include(i => i.User)
                .Include(i => i.Report)
                .FirstOrDefault(i => i.Id == id);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Get all investigations for user ID
    /// </summary>
    /// <param name="userId">ID of user that owns these investigations</param>
    /// <param name="closed">Determines if show also closed reports</param>
    /// <returns></returns>
    public IEnumerable<Report> GetAllMyInvestigations(string userId, bool closed = false)
    {
        try
        {
            var reports = _appDb.Reports
                .Include(r => r.User)
                .Include(r => r.Investigation)
                .Where(r => r.Investigation.UserId == userId);

            if (!closed)
            {
                reports = reports.Where(r => r.Status != ReportStatus.Closed);
            }

            return reports
                .OrderByDescending(r => r.CreatedDate);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Creates new investigation into database
    /// </summary>
    /// <param name="newInvestigation"></param>
    public void CreateInvestigation(Investigation newInvestigation)
    {
        try
        {
            _appDb.Investigations.Add(newInvestigation);
            _appDb.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Update investigation into database
    /// </summary>
    /// <param name="investigation"></param>
    public void UpdateInvestigation(Investigation investigation)
    {
        try
        {
            var existingInvestigation = _appDb.Investigations.FirstOrDefault(r => r.Id == investigation.Id);

            if (existingInvestigation != null)
            {
                existingInvestigation.Description = investigation.Description;
                existingInvestigation.ActionDate = investigation.ActionDate;

                _appDb.Entry(existingInvestigation).State = EntityState.Modified;
                _appDb.SaveChanges();
            }
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /**
     * +----------------------------------------------------------+
     * |                         UPVOTES                          |
     * +----------------------------------------------------------+
     */

    /// <summary>
    /// Gets the number of upvotes
    /// </summary>
    /// <param name="reportId"></param>
    /// <returns>Number of upvotes for report</returns>
    public int CountUpvotesForReport(int reportId)
    {
        try
        {
            return _appDb.Upvotes
                .Include(u => u.User)
                .Include(u => u.Report)
                .Where(u => u.ReportId == reportId)
                .Count();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Check if user has upvoted to report
    /// </summary>
    /// <param name="report"></param>
    /// <param name="userId">ID of user</param>
    /// <returns>True if user has upvoted otherwise false</returns>
    public bool UserHasUpvotedReport(Report report, string userId)
    {
        try
        {
            var upvoted = _appDb.Upvotes
                .Where(u => u.UserId == userId)
                .Where(u => u.ReportId == report.Id)
                .FirstOrDefault();

            return upvoted != null;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Creates the upvote record
    /// </summary>
    /// <param name="newUpvote"></param>
    public void CreateUpvote(Upvote newUpvote)
    {
        try
        {
            _appDb.Upvotes.Add(newUpvote);
            _appDb.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /// <summary>
    /// Deletes upvote from database
    /// </summary>
    /// <param name="reportId">ID of report</param>
    /// <param name="userId">ID of user who owns the upvote</param>
    public void DeleteUpvote(int reportId, string userId)
    {
        try
        {
            var upvote = _appDb.Upvotes
                .Where(u => u.UserId == userId)
                .Where(u => u.ReportId == reportId)
                .FirstOrDefault();

            if (upvote == null)
            {
                return;
            }

            _logger.LogCritical("Here");

            _appDb.Upvotes.Remove(upvote);
            _appDb.Entry(upvote).State = EntityState.Deleted;
            _appDb.SaveChanges();
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /**
     * +----------------------------------------------------------+
     * |                         ADMIN                            |
     * +----------------------------------------------------------+
     */

    /// <summary>
    /// Assign role to user
    /// </summary>
    /// <param name="userId">ID of user that will be assigned role</param>
    /// <param name="roleId">ID of role that will be assigned to user</param>
    /// <returns></returns>
    public async Task AssignRole(string userId, string roleId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId);

            if (user == null)
            {
                return;
            }

            var role = await _roleManager.FindByIdAsync(roleId);

            if (role == null)
            {
                return;
            }

            var roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles.ToArray());
            await _userManager.AddToRoleAsync(user, role.Name);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }

    /**
     * +----------------------------------------------------------+
     * |                       STATISTICS                         |
     * +----------------------------------------------------------+
     */

    /// <summary>
    /// 
    /// </summary>
    /// <returns></returns>
    public IEnumerable<Report> GetReportsForStatistics()
    {
        var currentYear = DateTime.Parse(DateTime.Now.Year + "-01-01 00:00");

        return _appDb.Reports
            .Include(r => r.User)
            .Where(r => r.CreatedDate > currentYear)
            .OrderByDescending(r => r.CreatedDate);
    }
}
