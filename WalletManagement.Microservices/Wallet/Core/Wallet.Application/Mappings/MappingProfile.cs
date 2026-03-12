using AutoMapper;
using Wallet.Application.Dtos;
using Wallet.Domain.Entities.Concretes;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.Application.Mappings
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<WalletEntity, WalletDto>().ReverseMap();

            CreateMap<WalletTransaction, WalletTransactionDto>().ReverseMap();

            CreateMap<DepositRequestDto, WalletTransaction>();

            CreateMap<WithdrawRequestDto, WalletTransaction>();

            CreateMap<TransferRequestDto, WalletTransaction>();
        }
    }
}
