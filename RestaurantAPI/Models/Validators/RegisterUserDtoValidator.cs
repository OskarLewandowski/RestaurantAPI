using FluentValidation;
using RestaurantAPI.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RestaurantAPI.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        private readonly RestaurantDbContext _dbContext;

        public RegisterUserDtoValidator(RestaurantDbContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .Equal(x => x.Password)
                .NotEmpty();

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = _dbContext.Users.Any(u => u.Email == value);

                    if (emailInUse == true)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });
        }
    }
}
