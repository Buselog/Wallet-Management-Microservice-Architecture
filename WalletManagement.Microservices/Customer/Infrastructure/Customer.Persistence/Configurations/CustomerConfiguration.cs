using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;

namespace Customer.Persistence.Configurations
{
    public class CustomerConfiguration : IEntityTypeConfiguration<CustomerEntity>
    {
        public void Configure(EntityTypeBuilder<CustomerEntity> builder)
        {
            builder.ToTable("Customers");

            builder.HasIndex(c => c.Email).IsUnique();

            builder.Property(c => c.FirstName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.LastName).IsRequired().HasMaxLength(50);
            builder.Property(c => c.Email).IsRequired().HasMaxLength(100);

            builder.Property(c => c.CustomerNo).IsRequired().HasMaxLength(20);
        }
    }
}
