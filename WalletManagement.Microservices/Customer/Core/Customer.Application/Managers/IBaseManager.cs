using Customer.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Managers
{
    public interface IBaseManager<D,T> where D : class, IEntity where T: class
    {
        Task<List<T>> GetAllAsync();
        Task<T?> GetByIdAsync(int id);
        Task<T?> FirstOrDefaultAsync(Expression<Func<D, bool>> exp);
        Task<string> AddAsync(T dto);
        Task<string> UpdateAsync(T dto);
        Task<string> DeleteAsync(int id);
    }
}
