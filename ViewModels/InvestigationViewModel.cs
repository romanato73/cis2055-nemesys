using cis2055_nemesys.Models;
using cis2055_nemesys.Models.Enums;

namespace cis2055_nemesys.ViewModels
{
	public class InvestigationViewModel
	{
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime ActionDate { get; set; }

        public UserViewModel User { get; set; }

        public ReportViewModel Report { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}

