using Newtonsoft.Json;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaFinanciero.WebApi.Models
{
    public class MetaFinanciera
    {
        public int Id { get; set; }
        [Column(TypeName = "varchar(30)")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public string Titulo { get; set; }
        [Column(TypeName = "money")]
        public double? MontoActual { get; set; }

        [Required(ErrorMessage = "Campo Obligatorio...")]
        [Column(TypeName = "money")]
        public double MontoFinal { get; set; }
        public bool Completada { get; set; }
        public bool Retirada { get; set; }
        public int UsuarioId { get; set; }
        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
    }

    public class MetaPayload
    {
        public int MetaId { get; set; }
        public double? Monto { get; set; }
    }
}
