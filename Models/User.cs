using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFinances.WebApi.Models
{
    public class User
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Required field...")]
        public string FirstName { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Required field...")]
        public string LastName { get; set; }
        [Column(TypeName = "varchar(30)")]
        [EmailAddress]
        public string Email { get; set; }
        [Column(TypeName = "varchar(80)")]
        public string Password { get; set; }
        [Required(ErrorMessage = "Required field...")]
        public bool IsAdmin { get; set; }
        public List<Transaction> Transactions { get; set; }
        public List<FinancialGoal> Goals { get; set; }
    }
}
