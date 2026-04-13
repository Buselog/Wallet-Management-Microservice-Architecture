using Microsoft.EntityFrameworkCore;
using MultiLanguage.Contract.Repositories;
using MultiLanguage.Domain.Entities;
using MultiLanguage.Persistence.Context;
using System.Linq.Expressions;

namespace MultiLanguage.Persistence.Repositories
{
    public class ResourceRepository : IResourceRepository
    {

        private readonly AppDbContext _context;
        private readonly DbSet<Resource> _dbSet;

        public ResourceRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Resources;
        }

        public async Task<List<Resource>> GetAllAsync()
        {
            return await _dbSet.Include(r => r.Language).ToListAsync();
        }

        public async Task<string> GetValueAsync(string key, string culture)
        {
            var resource = await _dbSet
                .Where(r => r.Key == key && r.Language.CultureCode == culture)
                .Select(r => r.Value)
                .FirstOrDefaultAsync();

            return resource ?? key;
        }

        public async Task<Dictionary<string, string>> GetAllByCultureCodeAsync(string culture)
        {
            return await _dbSet
                .Where(r => r.Language.CultureCode == culture)
                .Select(r=> new {r.Key, r.Value })
                .ToDictionaryAsync(r => r.Key, r => r.Value);
        }

        public async Task AddResourceAsync(Resource resource)
        {
            await _dbSet.AddAsync(resource);
        }

        public void UpdateResource(Resource resource)
        {
            _dbSet.Update(resource);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
