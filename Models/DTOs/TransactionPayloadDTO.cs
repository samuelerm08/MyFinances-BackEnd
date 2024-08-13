namespace MyFinances.WebApi.Models.DTOs
{
    public class TransactionPayloadDTO
    {
        public string UserId { get; set; }
        public double? AmountUpTo { get; set; }
        public string Date { get; set; }
        public TransactionType? TransactionType { get; set; }
        public bool? IsActive { get; set; }
    }
}
