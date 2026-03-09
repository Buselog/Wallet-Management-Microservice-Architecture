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

        public TransactionManager(ITransactionRepository transactionRepository, IMapper mapper) : base(transactionRepository, mapper)
        {
            _transactionRepository = transactionRepository;
        }

        public async Task<(List<WalletTransactionDto> Items, int TotalCount)> GetHistoryAsync(
           int walletId, DateTime? startDate, DateTime? endDate, int pageNumber, int pageSize)
        {
            var (entities, totalCount) = await _transactionRepository.GetFilteredHistoryAsync(
                walletId, startDate, endDate, pageNumber, pageSize);

            var dtos = _mapper.Map<List<WalletTransactionDto>>(entities);

            return (dtos, totalCount);
        }
    }
}
