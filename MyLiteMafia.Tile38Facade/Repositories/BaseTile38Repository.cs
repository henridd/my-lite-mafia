using GeoJSON.Net.Geometry;
using MyLiteMafia.Common.Models;
using StackExchange.Redis;
using System.Collections.Generic;
using System;
using Newtonsoft.Json;

namespace MyLiteMafia.Tile38Facade.Repositories
{
    public interface ITile38Repository<T>
    {
        Task<string> GetStoredDataAsync(T entity);
        Task StoreEntityCoordinatesAsync(T entity);
        Task<string> StoreAndGetDataAsync(T entity);
    }

    public abstract class BaseTile38Repository<T> : ITile38Repository<T>
        where T : Entity
    {
        private readonly IConnectionMultiplexer _redis;
        private readonly string _collection;
        private int pkCounter = 1;

        private IDatabase _database => _redis.GetDatabase();

        public BaseTile38Repository(IConnectionMultiplexer redis, string collection)
        {
            _redis = redis;
            _collection = collection;
        }

        public async Task StoreEntityCoordinatesAsync(T entity)
        {
            var json = JsonConvert.SerializeObject(entity.CoordinatesData);

            if(entity.RedisId == 0)
                entity.RedisId = pkCounter++;

            await _database.ExecuteAsync("SET", _collection, entity.RedisId.ToString(), "OBJECT", json);
        }

        public async Task<string> GetStoredDataAsync(T entity)
        {
            var result = await _database.ExecuteAsync(@"GET", _collection, entity.RedisId.ToString());

            return result?.ToString() ?? string.Empty;
        }

        public async Task<string> StoreAndGetDataAsync(T entity)
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
