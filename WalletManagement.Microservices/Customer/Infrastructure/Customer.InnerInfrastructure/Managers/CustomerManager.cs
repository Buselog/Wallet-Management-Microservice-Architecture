using AutoMapper;
using Customer.Application.Dtos;
using Customer.Application.Managers;
using Customer.Contract.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;

namespace Customer.InnerInfrastructure.Managers
{
    public class CustomerManager : BaseManager<CustomerEntity, CustomerDto>, ICustomerManager
    {
        private readonly ICustomerRepository _customerRepository;

        public CustomerManager(ICustomerRepository customerRepository, IMapper mapper) : base(customerRepository, mapper)
        {
            _customerRepository = customerRepository;
        }
    }
}
