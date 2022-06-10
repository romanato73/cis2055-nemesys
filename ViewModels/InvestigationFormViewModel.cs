using System.ComponentModel.DataAnnotations;
using cis2055_nemesys.Models.Enums;

namespace cis2055_nemesys.ViewModels
{
	public class InvestigationFormViewModel
	{
        public int Id { get; set; }

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(50)]
        [Display(Name = "Description")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Action Date is required.")]
        [Display(Name = "Action Date")]
        public string ActionDate { get; set; }

        [Required(ErrorMessage = "Action Time is required.")]
        [Display(Name = "Action Time")]
        public string ActionTime { get; set; }

        [Required(ErrorMessage = "Report ID is required.")]
        [Display(Name = "Report ID")]
        public int ReportId { get; set; }

        [Display(Name = "Report Status")]
        public ReportStatus ReportStatus { get; set; }
    }
}

