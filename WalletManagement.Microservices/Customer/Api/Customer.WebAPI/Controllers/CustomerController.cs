using Customer.Application.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Customer.WebAPI.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class CustomerController : ControllerBase
    {

        private readonly ICustomerManager _customerManager;

        public CustomerController(ICustomerManager customerManager)
        {
            _customerManager = customerManager;
        }

        [HttpGet("getCustomerNo-byPhone/{phone}")]
        public async Task<IActionResult> GetCustomerNoByPhone(string phone)
        {
            var customerNo = await _customerManager.GetCustomerNoByPhoneAsync(phone);

            return Ok(customerNo); 
        }


        [HttpGet("getCustomerName-byCustomerNo/{customerNo}")]
        public async Task<IActionResult> GetCustomerName(string customerNo)
        {
            var customer = await _customerManager.GetByCustomerNoAsync(customerNo);

            return Ok($"{customer.FirstName} {customer.LastName}");
        }

    }
}

