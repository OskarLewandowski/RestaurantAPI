using Microsoft.AspNetCore.Identity;
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
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountService(RestaurantDbContext context, IPasswordHasher<User> passwordHasher)
        {
            _context = context;
            _passwordHasher = passwordHasher;
        }

        public void RegisterUser(RegisterUserDto userDto)
        {
            var newUser = new User()
            {
                Email = userDto.Email,
                DateOfBirth = userDto.DateOfBirth,
                Nationality = userDto.Nationality,
                RoleId = userDto.RoleId,
            };

            var passwordHash = _passwordHasher.HashPassword(newUser, userDto.Password);

            newUser.PasswordHash = passwordHash;

            _context.Users.Add(newUser);
            _context.SaveChanges();
        }
    }
}
