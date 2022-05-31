using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using FluentAssertions;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Models;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Authorization.Policy;
using RestaurantAPI.IntergrationTests.Helpers;

namespace RestaurantAPI.IntergrationTests
{
    public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;

        public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(service =>
                    {
                        var dbContextOptions = service
                            .FirstOrDefault(service =>
                            service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        service.Remove(dbContextOptions);

                        service.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();
                        service.AddMvc(option => option.Filters.Add(new FakeUserFilter()));

                        service.AddDbContext<RestaurantDbContext>(options =>
                            options.UseInMemoryDatabase("RestaurantDbInMemory"));
                    });
                });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Delete_ForNonRestaurantOwner_ReturnsForbidden()
        {
            //arrange

            var restaurant = new Restaurant()
            {
                CreatedById = 900
            };

            //seed

            SeedRestaurant(restaurant);

            //act

            var response = await _client.DeleteAsync($"/api/restaurant/{restaurant.Id}");

            //asserts

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        [Fact]
        public async Task Delete_ForRestaurantOwner_ReturnsNoContent()
        {
            //arrange

            var restaurant = new Restaurant()
            {
                CreatedById = 1,
                Name = "Test restaurant"
            };

            //seed

            SeedRestaurant(restaurant);

            //act

            var response = await _client.DeleteAsync($"/api/restaurant/{restaurant.Id}");

            //asserts

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ForNonExistingRestaurant_ReturnsNotFound()
        {
            //arrange

            //act

            var response = await _client.DeleteAsync("/api/restaurant/999");

            //asserts

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task CreateRestaurant_WithValidModel_ReturnsCreatedStatus()
        {
            //arrange

            var model = new CreateRestaurantDto()
            {
                Name = "TestRestaurant",
                City = "Poznań",
                Street = "Korek 13"
            };

            var httpContent = model.ToJsonHttpContent();

            //act

            var response = await _client.PostAsync("/api/restaurant", httpContent);

            //asserts

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateRestaurant_WithInvalidModel_ReturnsBadRequest()
        {
            //arrange

            var model = new CreateRestaurantDto()
            {
                ContactEmail = "test@test.com",
                Description = "test desc",
                ContactNumber = "999 787 334"
            };

            var httpContent = model.ToJsonHttpContent();

            //act

            var response = await _client.PostAsync("/api/restaurant", httpContent);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Theory]
        [InlineData(1, 5)]
        [InlineData(1, 10)]
        [InlineData(1, 15)]
        [InlineData(2, 5)]
        public async Task GetAll_WithQueryParameters_ReturnsOkResult(int pageNumber, int pageSize)
        {
            //arrange


            //act

            var response = await _client.GetAsync($"/api/restaurant?pageNumber={pageNumber}&pageSize={pageSize}");

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }


        [Theory]
        [InlineData(1, 55)]
        [InlineData(11, 30)]
        [InlineData(1, 155)]
        [InlineData(-1, 155)]
        [InlineData(0, 0)]
        public async Task GetAll_WithInvalidQueryParameters_ReturnsBadRequest(int pageNumber, int pageSize)
        {
            //arrange


            //act

            var response = await _client.GetAsync($"/api/restaurant?pageNumber={pageNumber}&pageSize={pageSize}");

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        private void SeedRestaurant(Restaurant restaurant)
        {
            //seed
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContex = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContex.Restaurants.Add(restaurant);
            _dbContex.SaveChanges();
        }
    }
}
