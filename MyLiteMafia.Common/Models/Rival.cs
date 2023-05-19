using GeoJSON.Net.Geometry;

namespace MyLiteMafia.Common.Models
{
    public class Rival
    {
        public int RedisId { get; set; }

        public Point Coordinates { get; set; }

        public Rival(int id, double latitude, double longitude) :
            this(latitude, longitude)
        {
            RedisId = id;
        }

        public Rival(double latitude, double longitude)
        {
            Coordinates = new Point(new Position(latitude, longitude));
        }
    }
}
