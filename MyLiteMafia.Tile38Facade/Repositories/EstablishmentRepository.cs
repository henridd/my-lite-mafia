using MyLiteMafia.Common.Models;
using StackExchange.Redis;

namespace MyLiteMafia.Tile38Facade.Repositories
{
    public interface IEstablishmentRepository : ITile38Repository<Establishment>
    {

    }

    public class EstablishmentRepository : BaseTile38Repository<Establishment>, IEstablishmentRepository
    {
        public EstablishmentRepository(IConnectionMultiplexer redis) : base(redis, "establishments")
        {
        }
    }
}
