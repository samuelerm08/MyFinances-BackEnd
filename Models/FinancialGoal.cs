using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFinances.WebApi.Models
{
    public class FinancialGoal
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Required field...")]
        public string Title { get; set; }
        [Column(TypeName = "money")]
        public double? CurrentAmount { get; set; }
        [Required(ErrorMessage = "Required field...")]
        [Column(TypeName = "money")]
        public double FinalAmount { get; set; }
        public bool Completed { get; set; }
        public bool Withdrawn { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
    }

    public class GoalPayload
    {
        public int GoalId { get; set; }
        public double? Amount { get; set; }
    }
}