using System.Collections.Generic;
using System.Drawing;

namespace MapVizualizer
{
    internal class CityInfo
    {
        public IEnumerable<PointF> Vertices { get; set; }

        public Color Color { get; set; }
    }
}