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
            if (phone.Any(c => !char.IsDigit(c) && !" ()-+".Contains(c)))
            {
                throw new BaseBusinessException("ERR_PHONE_NUMBER_CONTAINS_INVALID_CHARACTERS");
            }

            var cleanedPhone = new string(phone.Where(char.IsDigit).ToArray());
            if (cleanedPhone.StartsWith("0")) cleanedPhone = cleanedPhone.Substring(1);

            if (cleanedPhone.Length != 10)
            {
                throw new BaseBusinessException("ERR_PHONE_NUMBER_MUST_CONTAIN_10_DIGITS");
            }

            var customerNo = await _customerRepository.GetCustomerNoByPhoneAsync(cleanedPhone);

            if (string.IsNullOrEmpty(customerNo))
            {
                throw new CustomerNotFoundException();
            }
            return customerNo;
        }
    }
}
