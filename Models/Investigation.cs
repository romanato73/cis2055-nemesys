using System;
namespace cis2055_nemesys.Models
{
    public class Investigation
    {
        public int Id { get; set; }
        public string Description { get; set; }
        public DateTime ActionDate { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
