using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyFinances.WebApi.Models
{
    public class Category
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Required field...")]
        public string Title { get; set; }        
    }
}
