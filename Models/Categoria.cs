using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaFinanciero.WebApi.Models
{
    public class Categoria
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public string Titulo { get; set; }        
    }
}
