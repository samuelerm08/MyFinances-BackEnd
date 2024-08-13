using System;

namespace MyFinances.WebApi.Models.DTOs
{
    public class TransactionDTO
    {
        public int Id { get; set; }
        public DateTime Date { get; set; }
        public string Details { get; set; }
        public double Amount { get; set; }
        public TransactionType TransactionType { get; set; }
        public Category Category { get; set; }
        public bool IsActive { get; set; }
    }
}
