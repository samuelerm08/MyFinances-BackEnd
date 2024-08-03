namespace SistemaFinanciero.WebApi.Models.DTOs
{
    public class TransaccionPayloadDTO
    {
        public string UserId { get; set; }
        public double? MontoHasta { get; set; }
        public string Fecha { get; set; }
        public TipoTransaccion? Tipo { get; set; }
        public bool? EstaActiva { get; set; }
    }
}
