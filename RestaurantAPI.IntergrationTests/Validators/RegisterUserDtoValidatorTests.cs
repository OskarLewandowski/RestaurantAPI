using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantAPI.IntergrationTests.Validators
{
    public class RegisterUserDtoValidatorTests
    {
        private readonly RestaurantDbContext _dbContext;

        public RegisterUserDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<RestaurantDbContext>();

            builder.UseInMemoryDatabase("DbContextInMemory");

            _dbContext = new RestaurantDbContext(builder.Options);

            Seed();
        }

        private void Seed()
        {
            var testUsers = new List<User>()
            {
                new User()
                {
                    Email = "test1@test.com"
                },

                new User()
                {
                    Email = "test2@test.com"
                }
            };

            _dbContext.Users.AddRange(testUsers);
            _dbContext.SaveChanges();
        }

        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<RegisterUserDto>()
            {
                new RegisterUserDto()
                {
                    Email = "test0@test.com",
                    Password = "ABCDEFG",
                    ConfirmPassword = "ABCDEFG"
                },

                new RegisterUserDto()
                {
                    Email = "test4@test.com",
                    Password = "password123",
                    ConfirmPassword = "password123"
                },

                new RegisterUserDto()
                {
                    Email = "test5@test.com",
                    Password = "busspaadd12",
                    ConfirmPassword = "busspaadd12",
                    Nationality = "Polish"
                }
            };

            return list.Select(q => new object[] { q });
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<RegisterUserDto>()
            {
                new RegisterUserDto()
                {
                    Email = "test0@test.com",
                    Password = "12345",
                    ConfirmPassword = "12345"
                },

                new RegisterUserDto()
                {
                    Email = "test2@test.com",
                    Password = "12345678",
                    ConfirmPassword = "12345678"
                },

                new RegisterUserDto()
                {
                    Email = "test111@test.com",
                    Password = "pa",
                    ConfirmPassword = "pa"
                },

                new RegisterUserDto()
                {
                    Email = "test5@test.com",
                    Password = "busspaadd12",
                    Nationality = "Polish"
                }
            };

            return list.Select(q => new object[] { q });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForValidModel_ReturnsSuccess(RegisterUserDto model)
        {
            //arrange

            var validator = new RegisterUserDtoValidator(_dbContext);

            //act

            var result = validator.TestValidate(model);

            //asserts

            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForInvalidModel_ReturnsFailure(RegisterUserDto model)
        {
            //arrange

            var validator = new RegisterUserDtoValidator(_dbContext);

            //act

            var result = validator.TestValidate(model);

            //asserts

            result.ShouldHaveAnyValidationError();
        }
    }
}
