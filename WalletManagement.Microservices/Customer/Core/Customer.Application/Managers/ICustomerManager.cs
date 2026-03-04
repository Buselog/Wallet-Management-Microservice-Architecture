using Customer.Application.Dtos;
using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;

namespace Customer.Application.Managers
{
    public interface ICustomerManager : IBaseManager<CustomerEntity, CustomerDto>
    {

    }
}
