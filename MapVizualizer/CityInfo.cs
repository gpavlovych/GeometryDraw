using System.Collections.Generic;
using System.Data.Entity.Spatial;
using System.Drawing;

namespace MapVizualizer
{
    internal class CityInfo
    {
        public CityAppearanceInfo Appearance { get; set; }

        public DbGeometry Geometry { get; internal set; }
    }
}