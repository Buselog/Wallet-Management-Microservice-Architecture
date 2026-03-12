using AutoMapper;
using Wallet.Application.Dtos;
using Wallet.Application.Factories;
using Wallet.Application.Helpers;
using Wallet.Application.Managers;
using Wallet.Contract.Repositories;
using Wallet.Domain.Entities.Enums;
using Wallet.Domain.Exceptions;
using Wallet.InnerInfrastructure.Services;
using WalletEntity = Wallet.Domain.Entities.Concretes.Wallet;

namespace Wallet.InnerInfrastructure.Managers
{
    public class WalletManager : BaseManager<WalletEntity, WalletDto>, IWalletManager
    {
        private readonly IWalletRepository _walletRepository;
        private readonly ITransactionRepository _transactionRepository;
        private readonly IWalletFactory _factory;
        private readonly CustomerServiceClient _customerService;
        public WalletManager(IWalletRepository walletRepository, ITransactionRepository transactionRepository, IMapper mapper, IWalletFactory factory, CustomerServiceClient customerService) : base(walletRepository, mapper)
        {
            _factory = factory;
            _walletRepository = walletRepository;
            _transactionRepository = transactionRepository;
            _customerService = customerService;
        }

        public async Task<decimal> GetBalanceAsync(int walletId, string customerNo)
        {
            await ValidateWalletOwnershipAsync(walletId, customerNo);
            var wallet = await _walletRepository.GetByIdAsync(walletId);

            return wallet!.Balance;
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

        public async Task<string> DepositAsync(DepositRequestDto dto, string customerNo)
        {
            await ValidateWalletOwnershipAsync(dto.WalletId, customerNo);
            return await ProcessTransactionAsync(dto.WalletId, dto.Amount, "Deposit", string.Empty, dto.ReferenceId);
        }

        public async Task<string> WithdrawAsync(WithdrawRequestDto dto, string customerNo)
        {
            await ValidateWalletOwnershipAsync(dto.WalletId, customerNo);
            return await  ProcessTransactionAsync(dto.WalletId, dto.Amount, "Withdraw", string.Empty, dto.ReferenceId);
        }

        public async Task<string> TransferAsync(TransferRequestDto dto, string customerNo)
        {
            await ValidateWalletOwnershipAsync(dto.FromWalletId, customerNo);
            var resolvedTarget = await ResolveTargetAddress(dto.Target);
            return await  ProcessTransactionAsync(dto.FromWalletId, dto.Amount, "Transfer", resolvedTarget, dto.ReferenceId);
        }

        private async Task ValidateWalletOwnershipAsync(int walletId, string customerNo)
        {
            var wallet = await _walletRepository.GetByIdAsync(walletId);
            if (wallet == null || !wallet.IsActive)
                throw new KeyNotFoundException("İşlem yapılmak istenen aktif cüzdan bulunamadı.");

            if (wallet.CustomerNo != customerNo)
                throw new UnauthorizedAccessException("Bu cüzdan üzerinde işlem yapma yetkiniz bulunmamaktadır!");
        }


        private async Task<string>  ProcessTransactionAsync(int walletId, decimal amount, string type, string target, string referenceId)
        {
            var exists = await _transactionRepository.ReferenceIdExistsAsync(referenceId);
            if (exists) throw new ReferenceAlreadyExistsException();

            var result = await _walletRepository.ExecuteMoneyTransactionWithSPAsync(walletId, amount, type, target, referenceId);

            return HandleSPResult(result);
        }

        private string HandleSPResult(int result)
        {
            return result switch
            {
                1 => "İşlem başarıyla tamamlandı.",
                0 => throw new InsufficientBalanceException(),
                -1 => throw new CustomerNotFoundException(),
                -2 => throw new Exception("Veritabanı seviyesinde bir hata oluştu (Rollback). Lütfen verileri kontrol edin."),
                -3 => throw new Exception("İşlem yapılmak istenen cüzdan bulunamadı veya pasif durumda."),
                _ => throw new Exception("İşlem sırasında sistemsel bir hata oluştu. Lütfen daha sonra tekrar deneyin.")
            };
        }

        private async Task<string> ResolveTargetAddress(string address)
        {
            if (address.StartsWith("TR"))
            {
                if (!IbanHelper.Validate(address))
                {
                    throw new InvalidIbanException();
                }
                return address;
            }

            if(string.IsNullOrWhiteSpace(address) || address.Length < 10)
            {
                throw new BaseBusinessException("Telefon numarası formatı hatalı.");
            }
            var customerNo = await GetCustomerNoByPhoneFromApi(address);
            if (string.IsNullOrEmpty(customerNo))
            {
                throw new CustomerNotFoundException();
            }

            return customerNo;
        }

        private async Task<string> GetCustomerNoByPhoneFromApi(string phoneNumber)
        {
            var customerNo = await _customerService.GetCustomerNoByPhoneAsync(phoneNumber);
            return customerNo!;
        }
    }
}

