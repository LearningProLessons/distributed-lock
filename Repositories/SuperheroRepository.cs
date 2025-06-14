using distributed_cache.Data;
using distributed_cache.Interfaces;

namespace distributed_cache.Repositories;

public class SuperheroRepository : ISuperheroRepository
{
    private readonly ApplicationDbContext _appDbContext;

    public SuperheroRepository(ApplicationDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
}