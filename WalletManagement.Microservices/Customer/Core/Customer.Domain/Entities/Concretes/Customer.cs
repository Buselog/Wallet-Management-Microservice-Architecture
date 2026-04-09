using Ardalis.GuardClauses;
using Customer.Domain.Entities.Abstracts;

namespace Customer.Domain.Entities.Concretes
{
    public class Customer : BaseEntity
    {
        public string CustomerNo { get; private set; } = string.Empty; 
        public string FirstName { get; private set; } = string.Empty;  
        public string LastName { get; private set; } = string.Empty;   
        public string PhoneNumber { get; private set; } = string.Empty; 
        public string Email { get; private set; } = string.Empty;      
        public string Password { get; private set; } = string.Empty;

        private Customer() { 
        
        }

        public Customer(string firstName, string lastName, string phoneNumber, string email, string password, string customerNo)
        {
            CustomerNo = Guard.Against.NullOrWhiteSpace(customerNo, nameof(customerNo), "ERR_CUSTOMERNO_CANNOT_EMPTY");
            FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName), "ERR_NAME_FIELD_CANNOT_BE_EMPTY");
            LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName), "ERR_SURNAME_FIELD_CANNOT_BE_EMPTY");
            Email = Guard.Against.NullOrWhiteSpace(email, nameof(email), "ERR_EMAIL_ADDRESS_CANNOT_EMPTY");
            Password = Guard.Against.NullOrWhiteSpace(password, nameof(password), "ERR_PASSWORD_CANNOT_EMPTY");

            Guard.Against.NullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
            if (phoneNumber.Length != 10 || !phoneNumber.All(char.IsDigit))
            {
                throw new ArgumentException("ERR_PHONE_NUMBER_MUST_CONTAIN_10_DIGITS", nameof(phoneNumber));
            }

            PhoneNumber = phoneNumber;
        }
    }
}

