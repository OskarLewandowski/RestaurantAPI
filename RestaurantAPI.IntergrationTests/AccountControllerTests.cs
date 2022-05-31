using FluentAssertions;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RestaurantAPI.Entities;
using RestaurantAPI.IntergrationTests.Helpers;
using RestaurantAPI.Models;
using RestaurantAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantAPI.IntergrationTests
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly Mock<IAccountService> _accountServiceMock = new Mock<IAccountService>();

        public AccountControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(service =>
                    {
                        var dbContextOptions = service
                            .FirstOrDefault(service =>
                            service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        service.Remove(dbContextOptions);

                        service.AddSingleton<IAccountService>(_accountServiceMock.Object);

                        service.AddDbContext<RestaurantDbContext>(options =>
                            options.UseInMemoryDatabase("RestaurantDbInMemory"));
                    });
                })
                .CreateClient();
        }

        [Fact]
        public async Task Login_ForRegisteredUser_ReturnOk()
        {
            //arrange

            _accountServiceMock
                .Setup(e => e.GenerateJwt(It.IsAny<LoginDto>()))
                .Returns("jwt");

            var loginDto = new LoginDto()
            {
                Email = "test@test.com",
                Password = "password123"
            };

            var httpContent = loginDto.ToJsonHttpContent();

            //act

            var response = await _client.PostAsync("/api/account/login", httpContent);

            //asserts

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterUser_ForValidModel_ReturnsOk()
        {
            //arrange

            var registerUser = new RegisterUserDto()
            {
                Email = "test@test.com",
                Password = "password123",
                ConfirmPassword = "password123"
            };

            var httpContent = registerUser.ToJsonHttpContent();

            //act

            var response = await _client.PostAsync("/api/account/register", httpContent);

            //asserts

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task RegisterUser_ForInvalidModel_ReturnsBadRequest()
        {
            //arrange

            var registerUser = new RegisterUserDto()
            {
                Password = "password123",
                ConfirmPassword = "12356"
            };

            var httpContent = registerUser.ToJsonHttpContent();

            //act

            var response = await _client.PostAsync("/api/account/register", httpContent);

            //asserts

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
