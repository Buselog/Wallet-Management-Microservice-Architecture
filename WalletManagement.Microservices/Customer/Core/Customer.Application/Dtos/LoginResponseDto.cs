using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Customer.Application.Dtos
{
    public class LoginResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public string CustomerNo { get; set; } = string.Empty;
        public string FullName { get; set; } = string.Empty;
    }
}
