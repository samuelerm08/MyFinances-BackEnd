using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SistemaFinanciero.WebApi.Models
{
    public class Balance
    {
        public int? Id { get; set; }
        public double Saldo_Total { get; set; }
        public int? TransaccionId { get; set; }
        public Transaccion Transaccion { get; set; }
    }
}
