using AutoMapper;
using Wallet.Application.Dtos;
using Wallet.Application.Factories;
using Wallet.Application.Helpers;
using Wallet.Application.Managers;
using Wallet.Contract.Repositories;
using Wallet.Domain.Entities.Enums;
using Wallet.Domain.Exceptions;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.InnerInfrastructure.Managers
{
    public class WalletManager : BaseManager<WalletEntity, WalletDto>, IWalletManager
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletFactory _factory;
        public WalletManager(IWalletRepository walletRepository, ITransactionRepository transactionRepository, IMapper mapper, IWalletFactory factory) : base(walletRepository, mapper)
        {
            _factory = factory;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
        }

        public async Task<decimal> GetBalanceAsync(int walletId, string customerNo)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId);

            if (wallet == null || !wallet.IsActive)
            {
                throw new KeyNotFoundException("Aktif cüzdan bulunamadı.");
            }

            if (wallet.CustomerNo != customerNo)
            {
                throw new UnauthorizedAccessException("Bu cüzdan bilgisine erişim yetkiniz yok.");
            }
               
            return wallet.Balance;
        }

        public async Task<List<WalletDto>> GetWalletsByCustomerNoAsync(string customerNo)
        {
            var wallets = await _walletRepository.GetWalletsByCustomerNoAsync(customerNo);
            return _mapper.Map<List<WalletDto>>(wallets);
        }

        public async Task<WalletDto> CreateNewWalletAsync(string customerNo, string currency, WalletType type)
        {
            var existing = await _walletRepository.GetWalletsByCustomerNoAsync(customerNo);
            if (existing.Any(x => x.Currency == currency && x.Type == type && x.IsActive))
                throw new BaseBusinessException($"{currency} - {type} tipinde aktif bir cüzdan zaten mevcut.");

            var newWallet = _factory.CreateWallet(customerNo, currency, type);

            await _walletRepository.AddAsync(newWallet);
            await _walletRepository.SaveChangesAsync();

            return _mapper.Map<WalletDto>(newWallet);
        }

        public async Task<string> ProcessTransactionAsync(CreateTransactionDto transactionDto, string type)
        {
            var exists = await _transactionRepository.ReferenceIdExistsAsync(transactionDto.ReferenceId);
            if (exists) throw new ReferenceAlreadyExistsException();

            if (transactionDto.TargetAddress.StartsWith("TR"))
            {
                if (!IbanHelper.Validate(transactionDto.TargetAddress))
                    throw new InvalidIbanException();
            }
            else
            {
                if (transactionDto.TargetAddress.Length < 10)
                    throw new BaseBusinessException("Telefon numarası formatı hatalı.");

                string targetCustomerNo = await GetCustomerNoByPhoneFromApi(transactionDto.TargetAddress);

                if (string.IsNullOrEmpty(targetCustomerNo))
                    throw new CustomerNotFoundException();

                transactionDto.TargetAddress = targetCustomerNo;
            }

            var result = await _walletRepository.ExecuteMoneyTransactionWithSPAsync(
                transactionDto.WalletId,
                transactionDto.Amount,
                type,
                transactionDto.TargetAddress,
                Guid.NewGuid().ToString() 
            );

            return result switch
            {
                1 => "İşlem başarıyla tamamlandı.",
                0 => throw new InsufficientBalanceException(),
                -1 => throw new CustomerNotFoundException(),
                _ => throw new Exception("İşlem sırasında sistemsel bir hata oluştu.")
            };
        }

        private async Task<string> GetCustomerNoByPhoneFromApi(string phoneNumber)
        {
            return "CUST12345";
        }
    }
}

