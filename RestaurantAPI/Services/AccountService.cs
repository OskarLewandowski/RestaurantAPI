using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto userDto);
    }

    public class AccountService : IAccountService
    {
        private readonly RestaurantDbContext _context;

        public AccountService(RestaurantDbContext context)
        {
            _context = context;
        }

        public void RegisterUser(RegisterUserDto userDto)
        {
            var newUser = new User()
            {
                Email = userDto.Email,
                PasswordHash = userDto.Password,
                DateOfBirth = userDto.DateOfBirth,
                Nationality = userDto.Nationality,
                RoleId = userDto.RoleId,
            };

            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
    }
}
