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

        public async Task<WalletDto> CreateNewWalletAsync(string currentCustomerNo, string currency, WalletType? type)
        {
            var walletType = type ?? 0;

            var allWallets = await _walletRepository.GetWalletsByCustomerNoAsync(currentCustomerNo, false);

            if (allWallets.Any(x => x.Currency == currency && x.Type == walletType && x.IsActive))
                throw new BaseBusinessException($"ERR_ACTIVE_WALLET_ALREADY_EXISTS | Detay: {currency} - {walletType}");

            int nextSuffix = allWallets.Any() ? allWallets.Max(x => x.Suffix) + 1 : 1;

            var newWallet = _factory.CreateWallet(currentCustomerNo, currency, walletType, nextSuffix);

            await _walletRepository.AddAsync(newWallet);
            await _walletRepository.SaveChangesAsync();

            return _mapper.Map<WalletDto>(newWallet);
        }

        public async Task DepositAsync(DepositRequestDto dto, string customerNo)
        {
            var walletId = dto.WalletId ?? 0;
            var amount = dto.Amount ?? 0;

            await ValidateWalletOwnershipAsync(walletId, customerNo);
            await ProcessTransactionAsync(walletId, amount, "Deposit", string.Empty, string.Empty, dto.ReferenceId);
        }

        public async Task WithdrawAsync(WithdrawRequestDto dto, string customerNo)
        {
            var walletId = dto.WalletId ?? 0;
            var amount = dto.Amount ?? 0;

            await ValidateWalletOwnershipAsync(walletId, customerNo);
            await  ProcessTransactionAsync(walletId, amount, "Withdraw", string.Empty, string.Empty, dto.ReferenceId);
        }

        public async Task TransferAsync(TransferRequestDto dto, string customerNo)
        {
            var fromWalletId = dto.FromWalletId ?? 0;
            var amount = dto.Amount ?? 0;

            await ValidateWalletOwnershipAsync(fromWalletId, customerNo);
            var resolvedTarget = await ResolveTargetAddress(dto.Target);
            await  ProcessTransactionAsync(fromWalletId, amount, "Transfer", resolvedTarget, dto.Description, dto.ReferenceId);
        }

        public async Task SoftDeleteWalletAsync(int walletId, string customerNo)
        {
            await ValidateWalletOwnershipAsync(walletId, customerNo);

            var wallet = await _walletRepository.GetByIdAsync(walletId);

            if(wallet.Balance != 0 || wallet.Balance != null)
            {
                throw new WalletBalanceIsNotEmptyExcepiton();
            }
                await _walletRepository.SoftDeleteWalletWithSPAsync(walletId, "USER_" + customerNo);
        }

        private async Task ValidateWalletOwnershipAsync(int walletId, string customerNo)
        {
            var wallet = await _walletRepository.GetByIdNoTrackingAsync(walletId);
            if (wallet == null || !wallet.IsActive)
                throw new WalletNotFoundException();

            if (wallet.CustomerNo != customerNo)
                throw new UnauthorizedAccessException("ERR_NO_PERMISSION_ON_WALLET");
        }

        private async Task ProcessTransactionAsync(int walletId, decimal amount, string type, string target, string description, string referenceId)
        {
            var exists = await _transactionRepository.ReferenceIdExistsAsync(referenceId);
            if (exists) throw new ReferenceAlreadyExistsException();

            string? senderName = null;

            if (type.Contains("Transfer", StringComparison.OrdinalIgnoreCase))
            {
                var wallet = await _walletRepository.GetByIdAsync(walletId);
                senderName = await _customerService.GetCustomerNameByCustomerNoAsync(wallet!.CustomerNo);
            }

            var result = await _walletRepository.ExecuteMoneyTransactionWithSPAsync(walletId, amount, type, target, description, referenceId, senderName);

            HandleSPResult(result);
        }

        private void HandleSPResult(int result)
        {
            if (result == 1) return;

            throw result switch
            {

                0 => new InsufficientBalanceException(),
                -1 => new CustomerNotFoundException(),
                -2 => new BaseBusinessException("ERR_DB_ROLLBACK"),
                -3 => new WalletNotFoundException(),
                _ => new Exception($"ERR_SYSTEM_TRANSACTION_FAILED | SP_Result: {result}")

            };
        }

        private async Task<string> ResolveTargetAddress(string address)
        {
            if (address.Trim().StartsWith("TR", StringComparison.OrdinalIgnoreCase))
            {
                var cleanIban = address.Replace(" ", "").ToUpper();
                if (!IbanHelper.Validate(cleanIban))
                {
                    throw new InvalidIbanException();
                }
                return cleanIban;
            }

            var cleanedPhone = new string(address.Where(char.IsDigit).ToArray());

            if (cleanedPhone.StartsWith("0")) cleanedPhone = cleanedPhone.Substring(1);

            if (cleanedPhone.Length != 10)
            {
                throw new BaseBusinessException("ERR_INVALID_PHONE_FORMAT");
            }

            var customerNo = await GetCustomerNoByPhoneFromApi(cleanedPhone);

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

