using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto restaurantDto);
        PagedResult<RestaurantDto> GetAll(RestaurantQuery query);
        RestaurantDto GetById(int id);
        void Delete(int id);
        void Update(UpdateRestaurantDto restaurantDto, int id);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IAuthorizationService _authorizationService;
        private readonly IUserContextService _userContextService;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper, ILogger<RestaurantService> logger
            , IAuthorizationService authorizationService, IUserContextService userContextService)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _logger = logger;
            _authorizationService = authorizationService;
            _userContextService = userContextService;
        }

        public void Update(UpdateRestaurantDto restaurantDto, int id)
        {
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
                 new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            restaurant.Name = restaurantDto.Name;
            restaurant.Description = restaurantDto.Description;
            restaurant.HasDelivery = restaurantDto.HasDelivery;

            _dbContext.SaveChanges();
        }

        public void Delete(int id)
        {
            _logger.LogError($"Restaurant with id: {id} DELETE action invoked");

            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            var authorizationResult = _authorizationService.AuthorizeAsync(_userContextService.User, restaurant,
                 new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidException();
            }

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            var restaurantsDto = _mapper.Map<RestaurantDto>(restaurant);

            return restaurantsDto;
        }

        public PagedResult<RestaurantDto> GetAll(RestaurantQuery query)
        {
            var baseQuery = _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .Where(r => query.SearchPharse == null ||
                      (r.Name.ToLower().Contains(query.SearchPharse.ToLower()) ||
                       r.Description.ToLower().Contains(query.SearchPharse.ToLower())));

            var restaurants = baseQuery
                .Skip(query.PageSize * (query.PageNumber - 1))
                .Take(query.PageSize)
                .ToList();

            var totalItemsCount = baseQuery.Count();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            var result = new PagedResult<RestaurantDto>(restaurantsDtos, totalItemsCount, query.PageSize, query.PageNumber);

            return result;
        }

        public int Create(CreateRestaurantDto restaurantDto)
        {
            var newRestaurant = _mapper.Map<Restaurant>(restaurantDto);
            newRestaurant.CreatedById = _userContextService.GetUserId;

            _dbContext.Restaurants.Add(newRestaurant);
            _dbContext.SaveChanges();

            return newRestaurant.Id;
        }

    }
}
