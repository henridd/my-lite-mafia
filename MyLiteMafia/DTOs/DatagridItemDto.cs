using System.Windows;

namespace MyLiteMafia.DTOs
{
    public class DatagridItemDto
    {
        public int RedisId { get; set; }

        public string Name { get; set; }

        public string Tile38Data { get; set; }

        public UIElement CanvasItem { get; set; }
    }
}
