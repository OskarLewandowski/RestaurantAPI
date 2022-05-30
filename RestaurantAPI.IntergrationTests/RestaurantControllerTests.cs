using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.VisualStudio.TestPlatform.TestHost;
using FluentAssertions;

namespace RestaurantAPI.IntergrationTests
{
    public class RestaurantControllerTests
    {
        [Theory]
        [InlineData(1, 5)]
        [InlineData(1, 10)]
        [InlineData(1, 15)]
        public async void GetAll_WithQueryParameters_ReturnsOkResult(int pageNumber, int pageSize)
        {
            //arrange

            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();

            //act

            var response = await client.GetAsync($"/api/restaurant?pageNumber={pageNumber}&pageSize={pageSize}");

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }


        [Theory]
        [InlineData(1, 55)]
        [InlineData(11, 30)]
        [InlineData(1, 155)]
        [InlineData(-1, 155)]
        [InlineData(0, 0)]
        public async void GetAll_WithInvalidQueryParameters_ReturnsBadRequest(int pageNumber, int pageSize)
        {
            //arrange

            var factory = new WebApplicationFactory<Startup>();
            var client = factory.CreateClient();

            //act

            var response = await client.GetAsync($"/api/restaurant?pageNumber={pageNumber}&pageSize={pageSize}");

            //assert

            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
