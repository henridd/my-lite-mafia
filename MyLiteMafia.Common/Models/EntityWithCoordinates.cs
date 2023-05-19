using GeoJSON.Net.Geometry;

namespace MyLiteMafia.Common.Models
{
    public abstract class Entity 
    {
        public int RedisId { get; set; }

        public abstract object CoordinatesData { get; }
    }
}
