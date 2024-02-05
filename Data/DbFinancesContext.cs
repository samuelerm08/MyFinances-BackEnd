using Microsoft.EntityFrameworkCore;
using SistemaFinanciero.WebApi.Models;

namespace SistemaFinanciero.WebApi.Data
{
    public class DbFinancesContext : DbContext
    {
        public DbFinancesContext(DbContextOptions<DbFinancesContext> options) : base(options) {  }

        public DbSet<Balance> Balance { get; set; }
        public DbSet<Categoria> Categoria { get; set; }
        public DbSet<MetaFinanciera> MetaFinanciera { get; set; }
        public DbSet<Transaccion> Transacciones { get; set; }
        public DbSet<Usuario> Usuario { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
           .Entity<Balance>()
           .HasOne(x => x.Transaccion)
           .WithOne(x => x.Balance)
           .HasForeignKey<Balance>(x => x.TransaccionId)
           .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
