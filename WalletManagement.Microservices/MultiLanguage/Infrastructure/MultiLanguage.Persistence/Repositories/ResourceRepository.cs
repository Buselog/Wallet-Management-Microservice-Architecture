using Microsoft.EntityFrameworkCore;
using MultiLanguage.Contract.Repositories;
using MultiLanguage.Domain.Entities;
using MultiLanguage.Persistence.Context;

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
                .Include(r => r.Language)
                .FirstOrDefaultAsync(r => r.Key == key && r.Language.CultureCode == culture);

            return resource?.Value ?? key;
        }

        public async Task<Dictionary<string, string>> GetAllByCultureCodeAsync(string culture)
        {
            return await _dbSet
                .Include(r => r.Language)
                .Where(r => r.Language.CultureCode == culture)
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
