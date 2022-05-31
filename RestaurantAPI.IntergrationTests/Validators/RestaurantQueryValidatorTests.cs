using FluentValidation.TestHelper;
using RestaurantAPI.Entities;
using RestaurantAPI.Models;
using RestaurantAPI.Models.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;

namespace RestaurantAPI.IntergrationTests.Validators
{
    public class RestaurantQueryValidatorTests
    {
        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<RestaurantQuery>()
            {
                new RestaurantQuery()
                {
                    PageNumber = 1,
                    PageSize = 10,
                },

                new RestaurantQuery()
                {
                    PageNumber = 2,
                    PageSize = 5,
                },

                new RestaurantQuery()
                {
                    PageNumber = 22,
                    PageSize = 5,
                    SortBy = nameof(Restaurant.Name),
                },

                new RestaurantQuery()
                {
                    PageNumber = 12,
                    PageSize = 15,
                    SortBy = nameof(Restaurant.Category),
                },
            };

            return list.Select(q => new object[] { q });
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<RestaurantQuery>()
            {
                new RestaurantQuery()
                {
                    PageNumber = 1,
                    PageSize = 17,
                },

                new RestaurantQuery()
                {
                    PageNumber = 0,
                    PageSize = 5,
                },

                new RestaurantQuery()
                {
                    PageNumber = 22,
                    PageSize = 54,
                    SortBy = nameof(Restaurant.ContactNumber),
                },

                new RestaurantQuery()
                {
                    PageNumber = 12,
                    PageSize = 15,
                    SortBy = nameof(Restaurant.ContactEmail),
                },
            };

            return list.Select(q => new object[] { q });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForCorrectModel_ReturnSuccess(RestaurantQuery model)
        {
            //arrange

            var validator = new RestaurantQueryValidator();

            //act

            //from FluentValidation.TestHelper;
            var result = validator.TestValidate(model);

            //asserts

            result.ShouldNotHaveAnyValidationErrors();
        }


        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForIncorrectModel_ReturnFailure(RestaurantQuery model)
        {
            //arrange

            var validator = new RestaurantQueryValidator();

            //act

            //from FluentValidation.TestHelper;
            var result = validator.TestValidate(model);

            //asserts

            result.ShouldHaveAnyValidationError();
        }

    }
}
