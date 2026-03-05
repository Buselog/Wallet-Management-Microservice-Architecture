using System.Linq.Expressions;
using Wallet.Domain.Entities.Abstract;

namespace Wallet.Application.Managers
{
    public interface IBaseManager<D, T> where D : class, IEntity where T : class
    {
        Task<List<T>> GetAllAsync();

        Task<T?> GetByIdAsync(int id);

        Task<T?> FirstOrDefaultAsync(Expression<Func<D, bool>> exp);

        Task<string> AddAsync(T dto);

        Task<string> UpdateAsync(T dto);

        Task<string> DeleteAsync(int id);
    }
}
