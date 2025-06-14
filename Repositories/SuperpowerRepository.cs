using distributed_cache.Data;
using distributed_cache.Interfaces;

namespace distributed_cache.Repositories;

public class SuperpowerRepository : ISuperpowerRepository
{
    private readonly ApplicationDbContext _appDbContext;

    public SuperpowerRepository(ApplicationDbContext appDbContext)
    {
        _appDbContext = appDbContext;
    }
}