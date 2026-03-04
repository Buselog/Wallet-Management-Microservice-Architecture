using Customer.Domain.Entities.Abstracts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Domain.Entities.Concretes
{
    public class Customer : BaseEntity
    {
        public string CustomerNo { get; set; } = string.Empty; 
        public string FirstName { get; set; } = string.Empty;  
        public string LastName { get; set; } = string.Empty;   
        public string PhoneNumber { get; set; } = string.Empty; 
        public string Email { get; set; } = string.Empty;      
        public string Password { get; set; } = string.Empty; 
    }
}

