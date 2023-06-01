using GeoJSON.Net.Geometry;
using Microsoft.Extensions.Configuration;
using MyLiteMafia.Common.Models;
using MyLiteMafia.Tile38Facade.Repositories;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MyLiteMafia.IntegrationTests
{
    public class Tests
    {
        private readonly IConfiguration _configuration;

        [Test]
        public async Task StoreEntityCoordinatesAsync_ShouldStoreCoordinates()
        {
            //Arrange
            var repository = await CreateRepositoryAsync();
            var rival = new Rival(1, 2);

            //Act
            var result = await repository.StoreAndGetDataAsync(rival);

            //Assert
            var point = JsonConvert.DeserializeObject<Point>(result);
            Assert.That(point.Coordinates.Latitude, Is.EqualTo(rival.Coordinates.Coordinates.Latitude));
            Assert.That(point.Coordinates.Longitude, Is.EqualTo(rival.Coordinates.Coordinates.Longitude));
        }

        private async Task<IRivalRepository> CreateRepositoryAsync()
        {
            var builder = new ConfigurationBuilder();
            builder.AddJsonFile(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json"));

            var root = builder.Build();

            var ipAddress= root["tile38ipaddress"];
            var connection = ConnectionMultiplexer.Connect(ipAddress);

            await connection.GetDatabase().ExecuteAsync("FLUSHDB");
            return new RivalRepository(connection);
        }
    }
}