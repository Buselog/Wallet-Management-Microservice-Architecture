using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Wallet.Domain.Entities.Concretes;

namespace Wallet.Persistence.Configurations
{
    public class TransactionConfigurations : IEntityTypeConfiguration<WalletTransaction>
    {
        public void Configure(EntityTypeBuilder<WalletTransaction> builder)
        {
            builder.HasKey(t => t.Id);
            builder.HasIndex(t => t.ReferenceId).IsUnique();

            builder.Property(t => t.Amount).HasPrecision(18, 4);
            builder.Property(t => t.TargetAddress).HasMaxLength(75);

            builder.HasOne(t => t.Wallet)
                   .WithMany(w => w.Transactions)
                   .HasForeignKey(t => t.WalletId);
                 
        }
    }
}
