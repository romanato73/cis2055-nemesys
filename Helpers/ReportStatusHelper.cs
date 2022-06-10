using cis2055_nemesys.Models.Enums;

namespace cis2055_nemesys.Helpers
{
    public static class ReportStatusHelper
	{
        public static string TranslateReportStatus(ReportStatus status)
        {
            if (status == ReportStatus.Open) return "Open";
            else if (status == ReportStatus.BeingInvestigated) return "Being Investigated";
            else if (status == ReportStatus.NoActionRequired) return "No Action Required";
            else if (status == ReportStatus.Closed) return "Closed";
            else return "Unknown";
        }
    }
}

