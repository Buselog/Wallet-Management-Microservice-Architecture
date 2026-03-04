using AutoMapper;
using Customer.Application.Managers;
using Customer.Contract.Repositories;
using Customer.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Customer.InnerInfrastructure.Managers
{
    public class BaseManager<D, T> : IBaseManager<D, T> where D: class, IEntity where T : class {

        protected readonly IBaseRepository<D> _repository;
        protected readonly IMapper _mapper;

        public BaseManager(IBaseRepository<D> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<T>> GetAllAsync()
        {
            var entities = await _repository.GetAllAsync();
            return _mapper.Map<List<T>>(entities);
        }

        public async Task<T?> GetByIdAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            return _mapper.Map<T>(entity);
        }

        public async Task<string> AddAsync(T dto)
        {
            var entity = _mapper.Map<D>(dto);

            await _repository.AddAsync(entity);
            await _repository.SaveChangesAsync();

            return "Kayıt başarıyla eklendi.";
        }

        public async Task<string> UpdateAsync(T dto)
        {
            var entity = _mapper.Map<D>(dto);
            _repository.Update(entity);
            await _repository.SaveChangesAsync();
            return "Güncelleme başarılı.";
        }

        public async Task<string> DeleteAsync(int id)
        {
            var entity = await _repository.GetByIdAsync(id);
            if (entity == null) return "Kayıt bulunamadı.";

            _repository.Delete(entity);
            await _repository.SaveChangesAsync();
            return "Kayıt silindi.";
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<D, bool>> exp)
        {
            var entities = await _repository.FindAsync(exp);
            var entity = entities.FirstOrDefault();
            return _mapper.Map<T>(entity);
        }

    }
}
