using System;

namespace SistemaFinanciero.WebApi.Models.DTOs
{
    public class TransaccionDTO
    {
        public int Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Detalle { get; set; }
        public double Monto { get; set; }
        public TipoTransaccion TipoTransaccion { get; set; }
        public Categoria Categoria { get; set; }
        public bool EstaActiva { get; set; }
    }
}
