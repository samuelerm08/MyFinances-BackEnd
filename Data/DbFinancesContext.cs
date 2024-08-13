using Microsoft.EntityFrameworkCore;
using MyFinances.WebApi.Models;

namespace MyFinances.WebApi.Data
{
    public class DbFinancesContext : DbContext
    {
        public DbFinancesContext(DbContextOptions<DbFinancesContext> options) : base(options) { }

        public DbSet<Balance> Balance { get; set; }
        public DbSet<Category> Categories { get; set; }
        public DbSet<FinancialGoal> FinancialGoals { get; set; }
        public DbSet<Transaction> Transactions { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder
           .Entity<Balance>()
           .HasOne(x => x.Transaction)
           .WithOne(x => x.Balance)
           .HasForeignKey<Balance>(x => x.TransactionId)
           .OnDelete(DeleteBehavior.Cascade);

            base.OnModelCreating(modelBuilder);
        }
    }
}
