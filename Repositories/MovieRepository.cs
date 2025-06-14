using distributed_cache.Data;
using distributed_cache.Interfaces;

namespace distributed_cache.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _appDbContext;

    public MovieRepository(ApplicationDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
}