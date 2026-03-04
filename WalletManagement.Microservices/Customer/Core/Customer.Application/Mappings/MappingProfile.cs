using AutoMapper;
using Customer.Application.Dtos;
using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;

namespace Customer.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<CustomerEntity, CustomerDto>().ReverseMap();

            CreateMap<CustomerRegisterDto, CustomerEntity>();
        }
    }
}
