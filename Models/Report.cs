using System;
using cis2055_nemesys.Models.Enums;

namespace cis2055_nemesys.Models
{
    public class Report
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public DateTime Spotted { get; set; }
        public string Description { get; set; }
        public ReportStatus Status { get; set; }
        public string? Image { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public ICollection<Upvote>? Upvotes { get; set; }

        public Investigation? Investigation { get; set; }

        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
}
