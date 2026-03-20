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
            CustomerNo = Guard.Against.NullOrWhiteSpace(customerNo, nameof(customerNo));
            FirstName = Guard.Against.NullOrWhiteSpace(firstName, nameof(firstName));
            LastName = Guard.Against.NullOrWhiteSpace(lastName, nameof(lastName));
            Email = Guard.Against.NullOrWhiteSpace(email, nameof(email));
            Password = Guard.Against.NullOrWhiteSpace(password, nameof(password));

            Guard.Against.NullOrWhiteSpace(phoneNumber, nameof(phoneNumber));
            if (phoneNumber.Length != 10 || !phoneNumber.All(char.IsDigit))
            {
                throw new ArgumentException("Telefon numarası tam 10 haneli rakamlardan oluşmalıdır.", nameof(phoneNumber));
            }

            PhoneNumber = phoneNumber;
        }
    }
}

