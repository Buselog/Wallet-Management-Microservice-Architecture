using Customer.Domain.Entities.Abstracts;
using Microsoft.EntityFrameworkCore;
using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;

namespace Customer.Persistence.Context
{
    public class CustomerContext : DbContext
    {
        public CustomerContext(DbContextOptions<CustomerContext> options) : base(options) {

        }

        public DbSet<CustomerEntity> Customers { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            var AllEntity = ChangeTracker.Entries<BaseEntity>();

            foreach (var entity in AllEntity)
            {
                if(entity.State == EntityState.Added)
                {
                    entity.Entity.CreatedDate = DateTime.Now;
                    entity.Entity.CreatedUserCode = "SYS";
                    entity.Entity.CreatedChannelCode = "SYS";
                    entity.Entity.CreatedTranCode = "SYS";
                }

                else if(entity.State == EntityState.Modified)
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
            modelBuilder.ApplyConfiguration(new Configurations.CustomerConfiguration());
            base.OnModelCreating(modelBuilder);
        }
    }
}
