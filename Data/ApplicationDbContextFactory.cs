using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace distributed_cache.Data;

public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
{
    public ApplicationDbContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();

        optionsBuilder.UseSqlServer("Server=localhost,1434;Database=MovieDb;User Id=sa;Password=Hello&Run1234;TrustServerCertificate=True;");

        return new ApplicationDbContext(optionsBuilder.Options);
    }
}