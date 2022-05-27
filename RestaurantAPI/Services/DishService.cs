using AutoMapper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Services
{
    public interface IDishService
    {
        int Create(int restaurantId, CreateDishDto dishDto);
        DishDto GetById(int restaurantId, int dishId);
        List<DishDto> GetAll(int restaurantId);
        void RemoveAll(int restaurantId);
        void RemoveById(int restaurantId, int dishId);
    }

    public class DishService : IDishService
    {
        private readonly RestaurantDbContext _context;
        private readonly IMapper _mapper;

        public DishService(RestaurantDbContext context, IMapper mapper)
        {
            _context = context;
            _mapper = mapper;
        }

        public int Create(int restaurantId, CreateDishDto dishDto)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var newDish = _mapper.Map<Dish>(dishDto);

            newDish.RestaurantId = restaurantId;

            _context.Dishes.Add(newDish);
            _context.SaveChanges();

            return newDish.Id;
        }

        public DishDto GetById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantById(restaurantId);

            var dish = _context.Dishes.FirstOrDefault(x => x.Id == dishId);

            if (dish == null || dish.RestaurantId != restaurantId)
            {
                throw new NotFoundException("Dish not found");
            }

            var dishDto = _mapper.Map<DishDto>(dish);

            return dishDto;
        }

        public List<DishDto> GetAll(int restaurantId)
        {
            var restaurant = GetRestaurantWithDishById(restaurantId);

            var dishDtos = _mapper.Map<List<DishDto>>(restaurant.Dishes);

            return dishDtos;
        }

        public void RemoveAll(int restaurantId)
        {
            var restaurant = GetRestaurantWithDishById(restaurantId);

            _context.RemoveRange(restaurant.Dishes);
            _context.SaveChanges();
        }

        public void RemoveById(int restaurantId, int dishId)
        {
            var restaurant = GetRestaurantWithDishById(restaurantId);

            var dishToRemove = restaurant.Dishes.FirstOrDefault(d => d.Id == dishId);

            if (dishToRemove == null || dishToRemove.RestaurantId != restaurantId)
            {
                throw new NotFoundException("Dish not found");
            }

            _context.Remove(dishToRemove);
            _context.SaveChanges();
        }

        private Restaurant GetRestaurantById(int restaurantId)
        {
            var restaurant = _context.Restaurants.FirstOrDefault(x => x.Id == restaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            return restaurant;
        }

        private Restaurant GetRestaurantWithDishById(int restaurantId)
        {
            var restaurant = _context.Restaurants
                .Include(r => r.Dishes)
                .FirstOrDefault(x => x.Id == restaurantId);

            if (restaurant == null)
            {
                throw new NotFoundException("Restaurant not found");
            }

            return restaurant;
        }
    }
}
