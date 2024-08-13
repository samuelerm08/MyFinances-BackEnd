using System.ComponentModel.DataAnnotations;

namespace MyFinances.WebApi.Models.Security
{
    public class UserAuth
    {
        [Required(ErrorMessage = "Required field...")]
        public string Email { get; set; }
        [Required(ErrorMessage = "Required field...")]
        public string Password { get; set; }
    }
}
