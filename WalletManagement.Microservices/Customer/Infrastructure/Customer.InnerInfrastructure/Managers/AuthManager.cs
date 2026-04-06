using Customer.Application.Dtos;
using Customer.Application.Managers;
using Customer.Contract.Repositories;
using Customer.Domain.Exceptions;
using Customer.InnerInfrastructure.Security;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using CustomerEntity = Customer.Domain.Entities.Concretes.Customer;

namespace Customer.InnerInfrastructure.Managers
{
    public class AuthManager : IAuthManager
    {
        private readonly ICustomerRepository _customerRepository;
        private readonly IConfiguration _configuration;
        private readonly IWalletServiceClient _walletClient;

        public AuthManager(ICustomerRepository customerRepository, IConfiguration configuration, IWalletServiceClient walletClient)
        {
            _customerRepository = customerRepository;
            _configuration = configuration;
            _walletClient = walletClient;
        }

        public async Task RegisterAsync(CustomerRegisterDto registerDto)
        {

            var cleanedPhone = new string(registerDto.PhoneNumber.Where(char.IsDigit).ToArray());
            if (cleanedPhone.StartsWith("0"))
            {
                cleanedPhone = cleanedPhone.Substring(1);
            }

            var isEmailExist = await _customerRepository.GetByEmailAsync(registerDto.Email);
            if (isEmailExist != null) throw new EmailAlreadyExistsException();

            var isPhoneExist = await _customerRepository.GetByPhoneAsync(cleanedPhone);
            if (isPhoneExist != null) throw new PhoneNumberAlreadyExistException();

            var passwordHash = PasswordHasher.HashPassword(registerDto.Password);

            var newCustomer = new CustomerEntity(registerDto.FirstName, registerDto.LastName, cleanedPhone, registerDto.Email, passwordHash, DateTime.Now.Ticks.ToString().Substring(10, 8));
          
            await _customerRepository.AddAsync(newCustomer);
            await _customerRepository.SaveChangesAsync();

            try
            {
                await _walletClient.CreateDefaultWalletAsync(newCustomer.CustomerNo);
            }
            catch (Exception ex)
            {
                Log.Error(ex, "Kayıt sonrası otomatik cüzdan oluşturulamadı. Müşteri No: {CustomerNo}", newCustomer.CustomerNo);
            }
        }

        public async Task<LoginResponseDto> LoginAsync(CustomerLoginDto loginDto)
        {
            var customer = await _customerRepository.GetByEmailAsync(loginDto.Email);

            if (customer == null || !PasswordHasher.VerifyPassword(loginDto.Password, customer.Password))
            {
                throw new InvalidCredentialsException();
            }

            var token = GenerateJwtToken(customer);

            return new LoginResponseDto
            {
                Token = token,
                CustomerNo = customer.CustomerNo,
                FullName = customer.FirstName + ' ' + customer.LastName
            };
        }

        private string GenerateJwtToken(CustomerEntity customer)
        {
            var claims = new[]
            {
              new Claim(ClaimTypes.NameIdentifier, customer.Id.ToString()),
              new Claim(ClaimTypes.Email, customer.Email),
              new Claim("CustomerNo", customer.CustomerNo)
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(3),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}
