using MyFinances.WebApi.Models.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFinances.WebApi.Models
{
    public enum TransactionType
    {
        Income, Expense, Reserve
    }
    public class Transaction
    {
        public int Id { get; set; }
        [Column(TypeName = "date")]
        [Required(ErrorMessage = "Required field...")]
        [DateTime]
        public DateTime Date { get; set; }
        [Column(TypeName = "varchar(80)")]
        [Required(ErrorMessage = "Required field...")]
        public string Details { get; set; }
        [Column(TypeName = "money")]
        [Required(ErrorMessage = "Required field...")]
        public double Amount { get; set; }
        [Column(TypeName = "varchar(10)")]
        [Required(ErrorMessage = "Required field...")]
        public TransactionType TransactionType { get; set; }
        [Required(ErrorMessage = "Required field...")]
        public int CategoryId { get; set; }
        [ForeignKey("CategoryId")]
        public Category Category { get; set; }
        public int? BalanceId { get; set; }
        [ForeignKey("BalanceId")]
        public Balance Balance { get; set; }
        public int UserId { get; set; }
        [ForeignKey("UserId")]
        public User User { get; set; }
        public bool IsActive { get; set; }
    }
}
