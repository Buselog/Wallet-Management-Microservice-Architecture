using Customer.Application.Dtos;
using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;

namespace Customer.Application.Managers
{
    public interface ICustomerManager : IBaseManager<CustomerEntity, CustomerDto>
    {
        Task<CustomerDto> GetByCustomerNoAsync(string customerNo);
        Task<CustomerDto> GetByEmailAsync(string email);
        Task<CustomerDto> GetByPhoneAsync(string phone);
        Task<string?> GetCustomerNoByPhoneAsync(string phone);
    }
}
