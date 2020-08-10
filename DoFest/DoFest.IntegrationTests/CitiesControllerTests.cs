﻿using DoFest.Business.Activities.Models.Places;
using DoFest.Entities.Activities.Places;
using DoFest.IntegrationTests.Shared.Extensions;
using DoFest.IntegrationTests.Shared.Factories;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using Xunit;

namespace DoFest.IntegrationTests
{
    public class CitiesControllerTests: IntegrationTests
    {
        [Fact]
        public async Task GetCitiesTest()
        {
            //Arrange

            // Act
            var response = await HttpClient.GetAsync("/api/v1/cities");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            var cities = await response.Content.ReadAsAsync<IList<City>>();
            cities.Should().HaveCount(1);
        }

        [Fact]
        public async Task CreateCityTest()
        {
            //Arrange
            var cityModel = new CreateCityModel()
            {
                Name = "Buzau"
            };

            // Act
            var response = await HttpClient.PostAsJsonAsync("/api/v1/cities", cityModel);

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            City city = null;
            await ExecuteDatabaseAction(async (doFestContext) => {
                city = await doFestContext.Cities.FirstAsync(entity => entity.Name == cityModel.Name);
            });
            city.Name.Should().NotBeNull();
        }

        [Fact]
        public async Task DeleteCityTest()
        {
            //Arrange
            var city = CityFactory.Default().WithName("Bacau");
            await ExecuteDatabaseAction(async (doFestContext) =>
            {
                await doFestContext.Cities.AddAsync(city);
                await doFestContext.SaveChangesAsync();
            });

            // Act
            var response = await HttpClient.DeleteAsync($"/api/v1/cities/{city.Id}");

            // Assert
            response.IsSuccessStatusCode.Should().BeTrue();
            City existingCity = null;
            await ExecuteDatabaseAction(async (doFestContext) => {
                existingCity = await doFestContext.Cities.FirstAsync(entity => entity.Name == city.Name);
            });
            existingCity.Name.Should().BeNull();
        }
    }
}
