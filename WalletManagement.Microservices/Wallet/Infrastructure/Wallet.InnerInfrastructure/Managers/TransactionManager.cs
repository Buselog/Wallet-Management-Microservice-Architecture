using AutoMapper;
using Wallet.Application.Dtos;
using Wallet.Application.Managers;
using Wallet.Contract.Repositories;
using Wallet.Domain.Entities.Concretes;

namespace Wallet.InnerInfrastructure.Managers
{
    public class TransactionManager : BaseManager<WalletTransaction, WalletTransactionDto>, ITransactionManager
    {

        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletRepository _walletRepository;

        public TransactionManager(ITransactionRepository transactionRepository, IMapper mapper, IWalletRepository walletRepository) : base(transactionRepository, mapper)
        {
            _transactionRepository = transactionRepository;
            _walletRepository = walletRepository;
        }

        public async Task<(List<WalletTransactionDto> Items, int TotalCount)> GetHistoryAsync(
           int walletId, string customerNo, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {

            var wallet = await _walletRepository.GetByIdNoTrackingAsync(walletId);

            if(wallet== null || !wallet.IsActive)
            {
                throw new KeyNotFoundException("İşlem yapılmak istenen aktif cüzdan bulunamadı.");
            }

            if (wallet.CustomerNo != customerNo)
                throw new UnauthorizedAccessException("Bu cüzdan üzerinde işlem yapma yetkiniz bulunmamaktadır!");


            var (entities, totalCount) = await _transactionRepository.GetFilteredHistoryAsync(
                walletId, startDate, endDate, pageNumber, pageSize);

            var dtos = _mapper.Map<List<WalletTransactionDto>>(entities);

            return (dtos, totalCount);
        }
    }
}
