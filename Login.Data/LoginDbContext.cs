using Login.Data.Interface;
using Login.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Login.Data
{
    public class LoginDbContext : DbContext, ILoginDbContext
    {
        public LoginDbContext(DbContextOptions<LoginDbContext> options) : base(options)
        {
        }

        public DbSet<Account> Account => Set<Account>();

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(GetType().Assembly);
            base.OnModelCreating(modelBuilder);
        }
    }
}
