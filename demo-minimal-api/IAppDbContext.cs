using Microsoft.EntityFrameworkCore;

namespace demo_minimal_api
{
    public interface IAppDbContext
    {
        DbSet<Article> Articles { get; set; }
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default(CancellationToken));
    }
}

