using GeoJSON.Net.Geometry;
using MyLiteMafia.Common.Events;
using MyLiteMafia.Common.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MyLiteMafia.Tile38Facade.Services
{
    public interface IGeofenceService
    {
        event EventHandler<GeofenceNotificationEventArgs> GeofenceNotificationReceived;
        Task CreateAndSubscribeGeofenceAsync(Establishment establishment);
        Task<string> CreateNewGeofenceAsync(Establishment establishment);
        Task SubscribeToGeofenceAsync(string channelName);
    }

    public class GeofenceService : IGeofenceService
    {
        private readonly IConnectionMultiplexer _redis;

        public event EventHandler<GeofenceNotificationEventArgs> GeofenceNotificationReceived;

        private IDatabase _database => _redis.GetDatabase();

        public GeofenceService(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task CreateAndSubscribeGeofenceAsync(Establishment establishment)
        {
            var channelName = await CreateNewGeofenceAsync(establishment);

            await SubscribeToGeofenceAsync(channelName);
        }

        public async Task<string> CreateNewGeofenceAsync(Establishment establishment)
        {
            var name = $"eg{establishment.Id}";
            await _database.ExecuteAsync("SETCHAN",
                name,
                "WITHIN",
                "rivals",
                "FENCE",
                "DETECT",
                "enter,exit",
                "OBJECT",
                JsonConvert.SerializeObject(establishment.Polygon)
                );

            return name;
        }

        public async Task SubscribeToGeofenceAsync(string channelName)
        {
            var channel = await _redis.GetSubscriber().SubscribeAsync(channelName);
            channel.OnMessage(x => GeofenceNotificationReceived?.Invoke(this, new GeofenceNotificationEventArgs(x.Message)));
        }
    }
}
