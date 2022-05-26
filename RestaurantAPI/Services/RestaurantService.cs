using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto restaurantDto);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto GetById(int id);
        bool Delete(int id);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly RestaurantDbContext _dbContext;
        private readonly IMapper _mapper;

        public RestaurantService(RestaurantDbContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }

        public bool Delete(int id)
        {
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);

            //false = not deleted
            if (restaurant == null)
            {
                return false;
            }

            _dbContext.Restaurants.Remove(restaurant);
            _dbContext.SaveChanges();

            return true;
        }

        public RestaurantDto GetById(int id)
        {
            var restaurant = _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
            {
                return null;
            }

            var restaurantsDto = _mapper.Map<RestaurantDto>(restaurant);

            return restaurantsDto;
        }

        public IEnumerable<RestaurantDto> GetAll()
        {
            var restaurants = _dbContext.Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .ToList();

            var restaurantsDtos = _mapper.Map<List<RestaurantDto>>(restaurants);

            return restaurantsDtos;
        }

        public int Create(CreateRestaurantDto restaurantDto)
        {
            var newRestaurant = _mapper.Map<Restaurant>(restaurantDto);

            _dbContext.Restaurants.Add(newRestaurant);
            _dbContext.SaveChanges();

            return newRestaurant.Id;
        }

    }
}
