namespace MyFinances.WebApi.Models
{
    public class Balance
    {
        public int? Id { get; set; }
        public double TotalBalance { get; set; }
        public int? TransactionId { get; set; }
        public Transaction Transaction { get; set; }
    }
}
