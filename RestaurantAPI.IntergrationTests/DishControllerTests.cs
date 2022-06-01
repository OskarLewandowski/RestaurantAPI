using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using System.Net.Http;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Authorization.Policy;

namespace RestaurantAPI.IntergrationTests
{
    public class DishControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {

        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Startup> _factory;

        public DishControllerTests(WebApplicationFactory<Startup> factory)
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

        [Theory]
        [InlineData(1)]
        [InlineData(2)]
        [InlineData(3)]
        [InlineData(4)]
        public async Task GetAll_WithValidQueryParameters_ReturnsOkResult(int restaurantId)
        {
            //arrange
            var restaurantsWithDishes = DataToSeedRestaurantWithDishes();
            SeedRestaurantWithDishes(restaurantsWithDishes);

            //act

            var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish");

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData(9999)]
        [InlineData(99999)]
        [InlineData(999999)]
        public async Task GetAll_WithInvalidQueryParameters_ReturnsNoFoundResult(int restaurantId)
        {
            //arrange

            //act

            var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish");

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }


        private IEnumerable<Restaurant> DataToSeedRestaurantWithDishes()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant()
                {
                    CreatedById = 1,
                    Name = "Restaurant test 1",
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Rt1 test dish 1",
                        },

                        new Dish()
                        {
                            Name = "Rt1 test dish 2",
                        },

                        new Dish()
                        {
                            Name = "Rt1 test dish 3",
                        },
                    },
                    Address = new Address()
                    {
                        City = "Poznań",
                        Street = "Szewska 1",
                        PostalCode = "11-222"
                    }
                },

                new Restaurant()
                {
                    CreatedById = 1,
                    Name = "Restaurant test 2",
                    Dishes = new List<Dish>()
                    {
                        new Dish()
                        {
                            Name = "Rt2 test dish 1",
                        },

                        new Dish()
                        {
                            Name = "Rt2 test dish 2",
                        },

                        new Dish()
                        {
                            Name = "Rt2 test dish 3",
                        },

                        new Dish()
                        {
                            Name = "Rt2 test dish 4",
                        },
                    },
                    Address = new Address()
                    {
                        City = "Wrocław",
                        Street = "Ronda 15",
                        PostalCode = "44-555"
                    }
                },
            };

            return restaurants;
        }

        private void SeedRestaurantWithDishes(IEnumerable<Restaurant> restaurants)
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContex = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContex.Restaurants.AddRange(restaurants);
            _dbContex.SaveChanges();
        }
    }
}
