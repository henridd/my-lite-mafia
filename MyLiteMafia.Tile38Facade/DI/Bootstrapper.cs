using Microsoft.Extensions.DependencyInjection;
using MyLiteMafia.Tile38Facade.Repositories;
using MyLiteMafia.Tile38Facade.Services;
using StackExchange.Redis;

namespace MyLiteMafia.Tile38Facade.DI
{
    public static class Bootstrapper
    {
        public static void RegisterDependencies(IServiceCollection services)
        {
            services.AddSingleton<IConnectionMultiplexer>(CreateConnectionMultiplexer());

            services.AddScoped<IRivalRepository, RivalRepository>();
            services.AddScoped<IEstablishmentRepository, EstablishmentRepository>();
            services.AddScoped<IGeofenceService, GeofenceService>();
        }

        private static IConnectionMultiplexer CreateConnectionMultiplexer()
        {
            var connection = ConnectionMultiplexer.Connect("localhost:9851");
            connection.GetDatabase().Execute("FLUSHDB");
            return connection;
        }
    }
}
