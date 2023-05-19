using MyLiteMafia.Common.Models;
using StackExchange.Redis;

namespace MyLiteMafia.Tile38Facade.Repositories
{
    public interface IRivalRepository : ITile38Repository<Rival>
    {
        
    }

    public class RivalRepository : BaseTile38Repository<Rival>, IRivalRepository
    {
        public RivalRepository(IConnectionMultiplexer connectionMultiplexer) : base(connectionMultiplexer, "rivals")
        {
        }
    }
}
