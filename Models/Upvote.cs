using System;

namespace cis2055_nemesys.Models
{
    public class Upvote
    {
        public int Id { get; set; }

        public string UserId { get; set; }
        public User User { get; set; }

        public int ReportId { get; set; }
        public Report Report { get; set; }
    }
}
