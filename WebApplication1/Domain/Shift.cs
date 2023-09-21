using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeClock.Domain
{
    public class Shift
    {
        [Key]
        public int Id { get; set; }
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public IdentityUser User { get; set; }

        public DateTime StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public DateTime? LunchStartTime { get; set; }
        public DateTime? LunchEndTime { get; set; }
        public DateTime? BreakStartTime { get; set; }
        public DateTime? BreakEndTime { get; set; }
        public double TotalHours { get; set; }
        public bool IsActive { get; set; }
    }
}
