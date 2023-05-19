using GeoJSON.Net.Geometry;

namespace MyLiteMafia.Common.Models
{
    public class Rival : Entity
    {
        public Point Coordinates { get; set; }

        public override object CoordinatesData => Coordinates;

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
