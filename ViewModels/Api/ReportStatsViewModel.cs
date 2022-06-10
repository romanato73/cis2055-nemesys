namespace cis2055_nemesys.ViewModels
{
    public class ReportStatsUser
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public int ReportCount { get; set; }
    }

	public class ReportStatsViewModel
	{
        public int TotalEntries { get; set; }

        public ReportStatsUser[] Users;
    }
}

