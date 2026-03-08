using Microsoft.EntityFrameworkCore;
using Wallet.Domain.Entities.Abstract;
using Wallet.Domain.Entities.Concretes;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Persistence.Context
{
    public class WalletContext : DbContext
    {
        public WalletContext(DbContextOptions<WalletContext> options): base(options)
        {

        }

        public DbSet<WalletEntity> Wallets { get; set; }
        public DbSet<WalletTransaction> WalletTransactions { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var AllEntity = ChangeTracker.Entries<BaseEntity>();

            foreach (var entity in AllEntity)
            {
                if (entity.State == EntityState.Added)
                {
                    entity.Entity.CreatedDate = DateTime.Now;
                    entity.Entity.CreatedUserCode = "SYS";
                    entity.Entity.CreatedChannelCode = "SYS";
                    entity.Entity.CreatedTranCode = "SYS";
                }

                else if (entity.State == EntityState.Modified)
                {
                    entity.Entity.LastUpdatedDate = DateTime.Now;
                    entity.Entity.LastUpdatedUserCode = "SYS";
                    entity.Entity.LastUpdatedChannelCode = "SYS";
                    entity.Entity.LastUpdatedTranCode = "SYS";
                }
            }

            return base.SaveChangesAsync(cancellationToken);
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletContext).Assembly);

            base.OnModelCreating(modelBuilder);
        }
    }
}
