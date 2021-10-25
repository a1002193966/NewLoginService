using System.Threading;
using System.Threading.Tasks;

namespace Login.Data.Interface
{
    public interface ILoginDbContext
    {
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
