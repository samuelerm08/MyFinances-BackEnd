namespace MyFinances.WebApi.Models.DTOs
{
    public class FinancialGoalDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public double FinalAmount { get; set; }
        public double CurrentAmount { get; set; }
        public bool Completed { get; set; }
        public bool Withdrawn { get; set; }
    }
}
