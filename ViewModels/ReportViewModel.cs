using cis2055_nemesys.Models;
using cis2055_nemesys.Models.Enums;

namespace cis2055_nemesys.ViewModels
{
	public class ReportViewModel
	{
        public int Id { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public DateTime Spotted { get; set; }
        public string Description { get; set; }
        public ReportStatus Status { get; set; }
        public string? Image { get; set; }

        public UserViewModel User { get; set; }

        public InvestigationViewModel? Investigation { get; set; }

        public bool UserHasUpvoted { get; set; }

        public int UpvotesCount { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

