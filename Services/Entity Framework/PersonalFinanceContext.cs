using Microsoft.EntityFrameworkCore;
using PersonalFinance.Models;


namespace PersonalFinance.Services.EntityFramework
{

    public class PersonalFinanceContext: DbContext
    {
        public PersonalFinanceContext(DbContextOptions<PersonalFinanceContext> options) : base
        (options)
        {

        }
        public DbSet<Transaction> Transaction { get; set; }
        public DbSet<Credit> Credit { get; set; }
        public DbSet<Debit> Debit { get; set; }
        public DbSet<KnownMovement> KnownMovement { get; set; }
        public DbSet<Bank> Bank { get; set; }
        public DbSet<Deposit> Deposit { get; set; }
        public DbSet<Ticket> Ticket { get; set; }


        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
        }
    }
}
