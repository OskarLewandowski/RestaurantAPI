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
using RestaurantAPI.Models;
using RestaurantAPI.IntergrationTests.Helpers;

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


        [Fact]
        public async Task DeleteById_ForRestaurantOwner_ReturnsNoContent()
        {
            //arrange

            var restaurantsWithDishes = DataToSeedRestaurantWithDishes();
            SeedRestaurantWithDishes(restaurantsWithDishes);

            var restaurantsList = restaurantsWithDishes.ToList();
            var restaurant = restaurantsList[0];
            var restaurantId = restaurant.Id;

            var restaurantDishesList = restaurant.Dishes.ToList();
            var dish = restaurantDishesList[0];
            var dishId = dish.Id;

            //act

            var response = await _client.DeleteAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            //asserts

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task DeleteById_ForNonRestaurantOwner_ReturnsNotFound()
        {
            //arrange

            var restaurantsWithDishes = DataToSeedRestaurantWithDishes();
            SeedRestaurantWithDishes(restaurantsWithDishes);

            var restaurantsList = restaurantsWithDishes.ToList();

            //restaurant one
            var restaurantOne = restaurantsList[0];
            var restaurantId = restaurantOne.Id;

            //restaurant two
            var restaurantTwo = restaurantsList[1];
            var restaurantDishesList = restaurantTwo.Dishes.ToList();
            var dish = restaurantDishesList[0];
            var dishId = dish.Id;

            //act

            var response = await _client.DeleteAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            //asserts

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Theory]
        [InlineData(0, 0)]
        [InlineData(0, 1)]
        [InlineData(0, 2)]
        [InlineData(1, 0)]
        [InlineData(1, 1)]
        [InlineData(1, 2)]
        [InlineData(1, 3)]
        public async Task GetById_WithValidQueryParameters_ReturnsOkResult(int restaurantNumber, int dishNumber)
        {
            //arrange
            var restaurantsWithDishes = DataToSeedRestaurantWithDishes();
            SeedRestaurantWithDishes(restaurantsWithDishes);

            var restaurantsList = restaurantsWithDishes.ToList();
            var restaurant = restaurantsList[restaurantNumber];
            var restaurantId = restaurant.Id;

            var restaurantDishesList = restaurant.Dishes.ToList();
            var dish = restaurantDishesList[dishNumber];
            var dishId = dish.Id;

            //act

            var response = await _client.GetAsync($"/api/restaurant/{restaurantId}/dish/{dishId}");

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Fact]
        public async Task GetById_WithInvalidQueryParameters_ReturnsNoFoundResult()
        {
            //arrange

            //act

            var response = await _client.GetAsync($"/api/restaurant/1/dish/9999");

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ForRestaurantOwner_ReturnsNoContent()
        {
            //arrange

            //seed
            var restaurantsWithDishes = DataToSeedRestaurantWithDishes();
            SeedRestaurantWithDishes(restaurantsWithDishes);

            var restaurantList = restaurantsWithDishes.ToList();
            var restaurantId = restaurantList[0].Id;

            //act

            var response = await _client.DeleteAsync($"/api/restaurant/{restaurantId}");

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
        public async Task CreateDish_WithValidModel_ReturnsCreatedStatus()
        {
            //arrange

            var model = new DishDto()
            {
                Name = "test value",
                Description = "test",
                Price = 1234,
            };

            var httpContent = model.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync($"/api/restaurant/1/dish/", httpContent);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateDish_WithInvalidModel_ReturnsBadRequest()
        {
            //arrange

            var model = new DishDto()
            {
                Description = "test",
                Price = 1234,
            };

            var httpContent = model.ToJsonHttpContent();

            //act
            var response = await _client.PostAsync($"/api/restaurant/1/dish/", httpContent);

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
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
