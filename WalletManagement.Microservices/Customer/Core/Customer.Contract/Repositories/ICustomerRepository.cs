using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;


namespace Customer.Contract.Repositories
{
    public interface ICustomerRepository : IBaseRepository<CustomerEntity>
    {
        Task<CustomerEntity> GetByCustomerNoAsync(string customerNo); 
        Task<CustomerEntity> GetByEmailAsync(string email);
        Task<CustomerEntity> GetByPhoneAsync(string phone);
        Task<string?> GetCustomerNoByPhoneAsync(string phone);
    }
}
