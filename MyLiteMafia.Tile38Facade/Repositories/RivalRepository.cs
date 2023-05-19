using MyLiteMafia.Common.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace MyLiteMafia.Tile38Facade.Repositories
{
    public interface IRivalRepository
    {
        Task<string> GetStoredDataAsync(Rival entity);
        Task StoreEntityCoordinatesAsync(Rival entity);
        Task<string> StoreAndGetDataAsync(Rival entity);
    }

    public class RivalRepository : IRivalRepository
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly string _collection = "rivals";
        private int pkCounter = 1;

        private IDatabase _database => _redis.GetDatabase();

        public RivalRepository(IConnectionMultiplexer redis)
        {
            _redis = redis;
        }

        public async Task StoreEntityCoordinatesAsync(Rival entity)
        {
            var json = JsonConvert.SerializeObject(entity.Coordinates);

            if (entity.RedisId == 0)
                entity.RedisId = pkCounter++;

            await _database.ExecuteAsync("SET", _collection, entity.RedisId.ToString(), "OBJECT", json);
        }

        public async Task<string> GetStoredDataAsync(Rival entity)
        {
            var result = await _database.ExecuteAsync(@"GET", _collection, entity.RedisId.ToString());

            return result?.ToString() ?? string.Empty;
        }

        public async Task<string> StoreAndGetDataAsync(Rival entity)
        {
            await StoreEntityCoordinatesAsync(entity);

            return await GetStoredDataAsync(entity);
        }

        protected IDatabase GetDatabase()
        {
            return _redis.GetDatabase();
        }
    }
}
