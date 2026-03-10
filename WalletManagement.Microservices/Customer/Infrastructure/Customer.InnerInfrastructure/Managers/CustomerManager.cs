using AutoMapper;
using Customer.Application.Dtos;
using Customer.Application.Managers;
using Customer.Contract.Repositories;
using Customer.Domain.Exceptions;
using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;

namespace Customer.InnerInfrastructure.Managers
{
    public class CustomerManager : BaseManager<CustomerEntity, CustomerDto>, ICustomerManager
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IMapper _mapper;

        public CustomerManager(ICustomerRepository customerRepository, IMapper mapper) : base(customerRepository, mapper)
        {
            _customerRepository = customerRepository;
            _mapper = mapper;
        }

        public async Task<CustomerDto> GetByCustomerNoAsync(string customerNo)
        {
            var customer = await _customerRepository.GetByCustomerNoAsync(customerNo);

            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> GetByEmailAsync(string email)
        {
            var customer = await _customerRepository.GetByEmailAsync(email);

            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<CustomerDto> GetByPhoneAsync(string phone)
        {
            var customer = await _customerRepository.GetByPhoneAsync(phone);

            if (customer == null)
            {
                throw new CustomerNotFoundException();
            }

            return _mapper.Map<CustomerDto>(customer);
        }

        public async Task<string> GetCustomerNoByPhoneAsync(string phone)
        {
            var customerNo = await _customerRepository.GetCustomerNoByPhoneAsync(phone);

            if (string.IsNullOrEmpty(customerNo))
            {
                throw new CustomerNotFoundException();
            }
            return customerNo;
        }
    }
}
