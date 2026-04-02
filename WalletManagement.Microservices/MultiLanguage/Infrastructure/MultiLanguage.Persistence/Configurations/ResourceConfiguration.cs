using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using MultiLanguage.Domain.Entities;

namespace MultiLanguage.Persistence.Configurations
{
    public class ResourceConfiguration : IEntityTypeConfiguration<Resource>
    {
        public void Configure(EntityTypeBuilder<Resource> builder)
        {
            builder.Property(r => r.Key).IsRequired().HasMaxLength(200);
            builder.Property(r => r.Value).IsRequired();

            builder.HasIndex(r => new { r.Key, r.LanguageId }).IsUnique();

            builder.HasOne(r => r.Language)
                   .WithMany(l => l.Resources)
                   .HasForeignKey(r => r.LanguageId);
        }
    }
}
