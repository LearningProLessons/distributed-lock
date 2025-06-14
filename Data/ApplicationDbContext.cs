using distributed_cache.Data.ContextConfigurations;
using distributed_cache.Model;
using Microsoft.EntityFrameworkCore;

namespace distributed_cache.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        // Generate three GUIDS and place them in an arrays
        var ids = new Guid[]
        {
            Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Guid.Parse("22222222-2222-2222-2222-222222222222"),
            Guid.Parse("33333333-3333-3333-3333-333333333333")
        };

        // Apply configuration for the three contexts in our application
        // This will create the demo data for our GraphQL endpoint.
        builder.ApplyConfiguration(new SuperheroContextConfiguration(ids));
        builder.ApplyConfiguration(new SuperpowerContextConfiguration(ids));
        builder.ApplyConfiguration(new MovieContextConfiguration(ids));
    }

    // Add the DbSets for each of our models we would like at our database
    public DbSet<Superhero> Superheroes { get; set; }
    public DbSet<Superpower> Superpowers { get; set; }
    public DbSet<Movie> Movies { get; set; }
}