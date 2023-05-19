using GeoJSON.Net.Geometry;
using MyLiteMafia.Common.Models;

namespace MyLiteMafia.Common.Factories
{
    public static class EstablishmentFactory
    {
        public static Establishment Create(int initialLatitude, int initiaLongitude, string name)
        {
            var positions = new List<IPosition>()
            {
                new Position(initialLatitude, initiaLongitude),
                new Position(initialLatitude, initiaLongitude + Establishment.Size),
                new Position(initialLatitude + Establishment.Size, initiaLongitude + Establishment.Size),
                new Position(initialLatitude + Establishment.Size, initiaLongitude),
                new Position(initialLatitude, initiaLongitude)
            };

            return new Establishment(positions, name);
        }
    }
}
