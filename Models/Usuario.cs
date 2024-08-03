using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;

namespace SistemaFinanciero.WebApi.Models
{
    public class Usuario
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public string Nombre { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public string Apellido { get; set; }
        [Column(TypeName = "varchar(30)")]
        [EmailAddress]
        public string Email { get; set; }
        [Column(TypeName = "varchar(80)")]
        public string Contraseña { get; set; }
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public bool EsAdmin { get; set; }
        public List<Transaccion> Transacciones { get; set; }
        public List<MetaFinanciera> Metas { get; set; }
    }
}
