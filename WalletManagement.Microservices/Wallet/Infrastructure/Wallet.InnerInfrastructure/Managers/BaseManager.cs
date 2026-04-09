using AutoMapper;
using System.Linq.Expressions;
using Wallet.Application.Managers;
using Wallet.Contract.Repositories;
using Wallet.Domain.Entities.Abstract;

namespace Wallet.InnerInfrastructure.Managers
{
    public class BaseManager<D, T> : IBaseManager<D, T> where D : class, IEntity where T : class
    {
        protected readonly IBaseRepository<D> _repository;
        protected readonly IMapper _mapper;
        public BaseManager(IBaseRepository<D> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var values = await _repository.GetAllAsync();
            return _mapper.Map<List<T>>(values);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var value = await _repository.GetByIdAsync(id);
            return _mapper.Map<T>(value);
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<D, bool>> exp)
        {
            var value = await _repository.FindAsync(exp);
            var entity = value.FirstOrDefault();
            return _mapper.Map<T>(entity);
        }

        public async Task AddAsync(T dto)
        {
            D domainEntity = _mapper.Map<D>(dto);
            await _repository.AddAsync(domainEntity);
            var result = await _repository.SaveChangesAsync();
        }

        public async Task UpdateAsync(T dto)
        {
            D domainEntity = _mapper.Map<D>(dto);
            _repository.Update(domainEntity);
            var result = await _repository.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {

            var value = await _repository.GetByIdAsync(id);
            if (value == null) throw new KeyNotFoundException("ERR_NO_RECORD_FOUND_TO_DELETE");

            _repository.Delete(value);
            var result = await _repository.SaveChangesAsync();
        }
    }
}
