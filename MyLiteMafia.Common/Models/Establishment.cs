using GeoJSON.Net.Geometry;

namespace MyLiteMafia.Common.Models
{
    public class Establishment : Entity
    {
        public static int Size = 15;

        public string Name { get; set; }

        public Polygon Polygon { get; set; }

        public override object CoordinatesData => Polygon;

        public Establishment(List<IPosition> positions, string name)
        {
            Name = name;
            Polygon = new Polygon(new List<LineString>()
            {
                new LineString(positions)
            });
        }
    }
}
