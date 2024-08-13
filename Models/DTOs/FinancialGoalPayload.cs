namespace MyFinances.WebApi.Models.DTOs
{
    public class FinancialGoalPayload
    {
        public int UserId { get; set; }
        public bool? Completed { get; set; }
    }
}
