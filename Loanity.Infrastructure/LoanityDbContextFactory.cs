using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Loanity.Infrastructure
{
    public class LoanityDbContextFactory : IDesignTimeDbContextFactory<LoanityDbContext>
    {
        public LoanityDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<LoanityDbContext>();

            // Use your actual connection string here
            optionsBuilder.UseSqlite("Data Source=loanity.db");

            return new LoanityDbContext(optionsBuilder.Options);
        }
    }
}
