using Customer.Contract.Repositories;
using Customer.Persistence.Context;
using Microsoft.EntityFrameworkCore;
using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;

namespace Customer.Persistence.Repositories
{
    public class CustomerRepository : BaseRepository<CustomerEntity>, ICustomerRepository
    {
        public CustomerRepository(CustomerContext context) : base(context) {
        
        }

        public async Task<CustomerEntity?> GetByCustomerNoAsync(string customerNo)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.CustomerNo == customerNo);
        }

        public async Task<CustomerEntity?> GetByEmailAsync(string email)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.Email == email);
        }

        public async Task<CustomerEntity?> GetByPhoneAsync(string phone)
        {
            return await _context.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phone);
        }

        public async Task<string> GetCustomerNoByPhoneAsync(string phone)
        {
            var customer = await _context.Customers.FirstOrDefaultAsync(x => x.PhoneNumber == phone);

            return customer.CustomerNo;
        }
    }
}
