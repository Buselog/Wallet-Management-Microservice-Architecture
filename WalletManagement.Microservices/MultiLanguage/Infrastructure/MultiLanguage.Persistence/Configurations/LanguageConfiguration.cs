using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiLanguage.Domain.Entities;

namespace MultiLanguage.Persistence.Configurations
{
    public class LanguageConfiguration : IEntityTypeConfiguration<Language>
    {
        public void Configure(EntityTypeBuilder<Language> builder)
        {
            builder.Property(x => x.Name).IsRequired().HasMaxLength(50);
            builder.Property(x => x.CultureCode).IsRequired().HasMaxLength(10);

            builder.HasData(
                new Language { Id = 1, Name = "Turkish", CultureCode = "tr-TR", IsDefault = true },
                new Language { Id = 2, Name = "English", CultureCode = "en-US", IsDefault = false }
            );

        }
    }
}
