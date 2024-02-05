namespace SistemaFinanciero.WebApi.Models.DTOs
{
    public class MetaFinancieraDTO
    {
        public int Id { get; set; }
        public string Titulo { get; set; }
        public double MontoFinal { get; set; }
        public double MontoActual { get; set; }
        public bool Completada { get; set; }
        public bool Retirada { get; set; }
    }
}
