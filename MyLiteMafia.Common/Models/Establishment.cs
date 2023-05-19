using GeoJSON.Net.Geometry;

namespace MyLiteMafia.Common.Models
{
    public class Establishment
    {
        public static int Size = 15;

        public int Id { get; set; }

        public string Name { get; set; }

        public Polygon Polygon { get; set; }

        public Establishment(int id, List<IPosition> positions, string name)
        {
            Id = id;
            Name = name;
            Polygon = new Polygon(new List<LineString>()
            {
                new LineString(positions)
            });
        }
    }
}
