using Microsoft.AspNetCore.Authorization;
using RestaurantAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Authorization
{
    public class CreatedMultipleRestaurantsRequirementHandler : AuthorizationHandler<CreatedMultipleRestaurantsRequirement>
    {
        private readonly RestaurantDbContext _restaurantDbContext;

        public CreatedMultipleRestaurantsRequirementHandler(RestaurantDbContext restaurantDbContext)
        {
            _restaurantDbContext = restaurantDbContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, CreatedMultipleRestaurantsRequirement requirement)
        {
            var userId = int.Parse(context.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier).Value);

            var createdRestaurantsCount = _restaurantDbContext.Restaurants.Count(r => r.CreatedById == userId);

            if (createdRestaurantsCount >= requirement.MinimumRestaurantsCreated)
            {
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
