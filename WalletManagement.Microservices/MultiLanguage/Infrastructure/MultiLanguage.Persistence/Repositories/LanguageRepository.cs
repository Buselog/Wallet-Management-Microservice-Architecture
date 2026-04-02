using Microsoft.EntityFrameworkCore;
using MultiLanguage.Contract.Repositories;
using MultiLanguage.Domain.Entities;
using MultiLanguage.Persistence.Context;

namespace MultiLanguage.Persistence.Repositories
{
    public class LanguageRepository : ILanguageRepository
    {
        private readonly AppDbContext _context;
        private readonly DbSet<Language> _dbSet;

        public LanguageRepository(AppDbContext context)
        {
            _context = context;
            _dbSet = _context.Languages;
        }

        public async Task<List<Language>> GetAllLanguagesAsync()
        {
           var values = await _dbSet.AsNoTracking().ToListAsync();
            return values;
        }

        public async Task<Language> GetByCultureCodeAsync(string cultureCode)
        {
           return await _dbSet.Where(x => x.CultureCode == cultureCode).FirstOrDefaultAsync();
        }

        public async Task AddLanguageAsync(Language language)
        {
            await _dbSet.AddAsync(language);
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }
    }
}
