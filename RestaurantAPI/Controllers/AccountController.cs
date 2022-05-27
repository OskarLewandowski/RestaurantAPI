using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginDto loginDto)
        {
            string token = _accountService.GenerateJwt(loginDto);

            return Ok(token);
        }

        [HttpPost("register")]
        public ActionResult RegisterUser([FromBody] RegisterUserDto userDto)
        {
            _accountService.RegisterUser(userDto);

            return Ok();
        }
    }
}
