using GeoJSON.Net.Geometry;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Windows.Shapes;
using Point = System.Windows.Point;
using Polygon = System.Windows.Shapes.Polygon;

namespace MyLiteMafia
{
    public static class ShapesConverter
    {
        public static Rectangle Convert(GeoJSON.Net.Geometry.Point source)
        {
            return new Rectangle()
            {
                Stroke = Brushes.Red,
                RadiusX = 2,
                RadiusY = 2,
                StrokeThickness = 2,
                Margin = new Thickness(source.Coordinates.Longitude, source.Coordinates.Latitude, 0, 0),
            };
        }

        public static Polygon Convert(GeoJSON.Net.Geometry.Polygon source)
        {
            var coordinates = source.Coordinates.First();

            return new Polygon()
            {
                Fill = Brushes.LightGray,
                Stroke = Brushes.Black,
                Points = new PointCollection(coordinates.Coordinates.Select(x => Convert(x)))
            };
        }

        private static Point Convert(IPosition position)
            => new Point(position.Longitude, position.Latitude);
    }
}
