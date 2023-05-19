using GeoJSON.Net.Geometry;
using MyLiteMafia.Common.Events;
using StackExchange.Redis;

namespace MyLiteMafia.Tile38Facade.Services
{
    public interface IGeofenceService
    {
        event EventHandler<GeofenceNotificationEventArgs> GeofenceNotificationReceived;
        Task CreateAndSubscribeGeofenceAsync(int establishmentId, IPosition southwesternPoint, IPosition northeasternPoint);
        Task<string> CreateNewGeofenceAsync(int establishmentId, IPosition southwesternPoint, IPosition northeasternPoint);
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

        public async Task CreateAndSubscribeGeofenceAsync(int establishmentId, IPosition southwesternPoint, IPosition northeasternPoint)
        {
            var channelName = await CreateNewGeofenceAsync(establishmentId, southwesternPoint, northeasternPoint);

            await SubscribeToGeofenceAsync(channelName);
        }

        public async Task<string> CreateNewGeofenceAsync(int establishmentId, IPosition southwesternPoint, IPosition northeasternPoint)
        {
            var name = $"eg{establishmentId}";
            await _database.ExecuteAsync("SETCHAN",
                name,
                "WITHIN",
                "rivals",
                "FENCE",
                "DETECT",
                "enter,exit",
                "BOUNDS",
                southwesternPoint.Latitude,
                southwesternPoint.Longitude,
                northeasternPoint.Latitude,
                northeasternPoint.Longitude);

            return name;
        }

        public async Task SubscribeToGeofenceAsync(string channelName)
        {
            var channel = await _redis.GetSubscriber().SubscribeAsync(channelName);
            channel.OnMessage(x => GeofenceNotificationReceived?.Invoke(this, new GeofenceNotificationEventArgs(x.Message)));
        }
    }
}
