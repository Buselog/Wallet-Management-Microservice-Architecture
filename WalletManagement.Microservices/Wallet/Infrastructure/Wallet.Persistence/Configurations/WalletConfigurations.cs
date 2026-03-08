using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Persistence.Configurations
{
    public class WalletConfigurations : IEntityTypeConfiguration<WalletEntity>
    {
        public void Configure(EntityTypeBuilder<WalletEntity> builder)
        {

            builder.HasKey(w => w.Id);
            builder.HasIndex(w => w.IBAN).IsUnique();
            builder.HasIndex(w => new { w.CustomerNo, w.Currency, w.Type }).IsUnique();

            builder.Property(w => w.Balance).HasPrecision(18, 4); 
            builder.Property(w => w.CustomerNo).IsRequired().HasMaxLength(50);
            builder.Property(w => w.Currency).IsRequired().HasMaxLength(3);

            builder.Property(w => w.RowVersion).IsRowVersion();


        }
    }
}
