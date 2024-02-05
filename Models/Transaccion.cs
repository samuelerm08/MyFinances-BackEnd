using SistemaFinanciero.WebApi.Models.Helpers;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaFinanciero.WebApi.Models
{
    public enum TipoTransaccion
    {
        Ingreso, Egreso, Reserva
    }
    public class Transaccion
    {
        public int Id { get; set; }  
        
        [Column(TypeName = "date")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        [DateTime]
        public DateTime Fecha { get; set; }   
        
        [Column(TypeName = "varchar(80)")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public string Detalle { get; set; }

        [Column(TypeName = "money")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public double Monto { get; set; }

        [Column(TypeName = "varchar(10)")]
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public TipoTransaccion TipoTransaccion { get; set; }
        [Required(ErrorMessage = "Campo Obligatorio...")]
        public int Cat_Id { get; set; }

        [ForeignKey("Cat_Id")]
        public Categoria Categoria { get; set; }        
        public int? Balance_Id { get; set; }

        [ForeignKey("Balance_Id")]
        public Balance Balance { get; set; }

        public int UsuarioId { get; set; }

        [ForeignKey("UsuarioId")]
        public Usuario Usuario { get; set; }
        public bool EstaActiva { get; set; }
    }
}
