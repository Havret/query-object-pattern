using BasketService.Infrastructure.Db;

namespace BasketService.Infrastructure;

public class BasketDbContext : BaseDbContext
{
    public BasketDbContext(string connectionString) : base(connectionString)
    {
    }
}