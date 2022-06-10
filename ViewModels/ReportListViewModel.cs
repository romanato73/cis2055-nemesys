using System.ComponentModel.DataAnnotations;
using cis2055_nemesys.Models;
#nullable disable

namespace cis2055_nemesys.ViewModels
{
	public class ReportListViewModel
	{
		public int TotalEntries { get; set; }

		public IEnumerable<ReportViewModel> Reports { get; set; }
	}
}

