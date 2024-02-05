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
        //Se permiten dos nombres. El segundo es opcional.
        [RegularExpression(@"^[A-Za-z]{1,15}( [A-Za-z]{1,15})?$", ErrorMessage = "El máximo permitido por nombre es de 15 caracteres.")]
        public string Nombre { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        //Se permiten dos apellidos. El segundo es opcional.
        [RegularExpression(@"^[A-Za-z]{1,15}( [A-Za-z]{1,15})?$", ErrorMessage = "El máximo permitido por apellido es de 15 caracteres.")]
        public string Apellido { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        [RegularExpression(@"^[A-Za-z0-9._-]{1,15}@[A-Za-z0-9.-]{1,15}\.[A-Za-z]{2,5}(\.[A-Za-z]{2,5})?$", ErrorMessage = "La dirección de correo electrónico no es válida.")]
        public string Email { get; set; }
        [Column(TypeName = "varchar(80)")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public string Contraseña { get; set; }
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public bool EsAdmin { get; set; }
        public List<Transaccion> Transacciones { get; set; }
        public List<MetaFinanciera> Metas { get; set; }
    }
}
