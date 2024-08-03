using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SistemaFinanciero.WebApi.Models.Security
{
    public class UsuarioAuth
    {        
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public string Email { get; set; }        
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public string Contraseña { get; set; }
    }
}
