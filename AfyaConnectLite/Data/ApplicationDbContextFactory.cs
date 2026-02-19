using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using AfyaConnectLite.Models;

namespace AfyaConnectLite.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionsBuilder.UseSqlite("Data Source=afyaconnect.db");

            return new ApplicationDbContext(optionsBuilder.Options);
        }
    }
}
