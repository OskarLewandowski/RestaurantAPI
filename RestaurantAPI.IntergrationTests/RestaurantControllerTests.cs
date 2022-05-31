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

namespace RestaurantAPI.IntergrationTests
{
    public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Startup>>
    {
        private readonly HttpClient _client;

        public RestaurantControllerTests(WebApplicationFactory<Startup> factory)
        {
            _client = factory.CreateClient();
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
    }
}
