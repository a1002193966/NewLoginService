using System.Threading;
using System.Threading.Tasks;
using Login.DomainModel;
using Microsoft.EntityFrameworkCore;

namespace Login.Data.Interface
{
    public interface ILoginDbContext
    {
        DbSet<Account> Account { get; }

        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
