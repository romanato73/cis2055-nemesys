using cis2055_nemesys.Models.Enums;

namespace cis2055_nemesys.Models.Interfaces
{
	public interface INemesysRepository
	{
		// Reports
		IEnumerable<Report> GetAllReports(bool closed = false);
		IEnumerable<Report> GetAllMyReports(string userId, bool closed = false);
		Report? GetReportById(int id);
		void CreateReport(Report newReport);
		void UpdateReport(Report report);
		void UpdateReportStatus(int id, ReportStatus newtatus);
		void DeleteReport(Report report);
		bool UserHasUpvotedReport(Report report, string userId);

		// Investigations
		Investigation? GetInvestigationByReportId(int reportId);
		Investigation? GetInvestigationById(int id);
		IEnumerable<Report> GetAllMyInvestigations(string userId, bool closed = false);
		void CreateInvestigation(Investigation newInvestigation);
		void UpdateInvestigation(Investigation investigation);

		// Upvotes
		int CountUpvotesForReport(int reportId);
		void CreateUpvote(Upvote newUpvote);
		void DeleteUpvote(int reportId, string userId);

		// Admin
		Task AssignRole(string userId, string Role);
	}
}

