using System.ComponentModel.DataAnnotations;
#nullable disable

namespace cis2055_nemesys.ViewModels
{
	public class ReportEditViewModel
	{
		public int Id { get; set; }

		[Required(ErrorMessage = "Location is required.")]
		[StringLength(50)]
		[Display(Name = "Report Location")]
		public string Location { get; set; }

		public DateTime Spotted { get; set; }

		[Required(ErrorMessage = "Date is required.")]
		[Display(Name = "Report Date")]
		public string Date { get; set; }

		[Required(ErrorMessage = "Time is required.")]
		[Display(Name = "Report Time")]
		public string Time { get; set; }

		[Required(ErrorMessage = "Type is required.")]
		[StringLength(100)]
		[Display(Name = "Report Type")]
		public string Type { get; set; }

		[Required(ErrorMessage = "Description is required.")]
		[StringLength(1500)]
		[Display(Name = "Report Description")]
		public string Description { get; set; }

		[Display(Name = "Report Image")]
		public IFormFile Image { get; set; }

		public string ImageUrl { get; set; }
	}
}

