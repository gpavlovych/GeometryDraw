using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using TestGeometryDraw;

namespace MapVizualizer
{
    public static class MapHelper
    {
        /// <summary>
        /// Draws the map of cities provided in the SQL database.
        /// </summary>
        /// <param name="imageSize">Size of the image.</param>
        /// <param name="paddingLeft">The padding left.</param>
        /// <param name="paddingRight">The padding right.</param>
        /// <param name="paddingTop">The padding top.</param>
        /// <param name="paddingBottom">The padding bottom.</param>
        /// <param name="backgroundColor">Color of the background.</param>
        /// <param name="legendItems">The legend items.</param>
        /// <param name="legendCaption">The legend caption.</param>
        /// <param name="legendFont">The legend font.</param>
        /// <param name="legendFontColor">Color of the legend font.</param>
        /// <param name="lineSpacing">The line spacing.</param>
        /// <returns></returns>
        public static Image DrawMap(
            Size imageSize,
            int paddingLeft,
            int paddingRight,
            int paddingTop,
            int paddingBottom,
            Color backgroundColor,
            IDictionary<Color, string> legendItems,
            string legendCaption,
            Font legendFont,
            Color legendFontColor,
            float lineSpacing)
        {

            var result = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(result))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(backgroundColor);
                using (var geoEntitiesContext = new geoEntities())
                {
                    var cityInfos = GetCityInfos(geoEntitiesContext);
                    DrawCities(graphics, paddingLeft, paddingRight, paddingTop, paddingBottom, cityInfos);
                    DrawLegend(graphics, paddingLeft, paddingTop, legendItems, legendCaption, legendFont, legendFontColor, lineSpacing);
                }
            }
            return result;
        }

        private static void DrawCities(
            Graphics graphics,
            int paddingLeft,
            int paddingRight,
            int paddingTop,
            int paddingBottom,
            IList<CityInfo> cityInfos)
        {
            var allVertices = cityInfos.SelectMany(cityVertices => cityVertices.Vertices).ToList();
            var xMax = allVertices.Max(point => point.X);
            var xMin = allVertices.Min(point => point.X);
            var yMax = allVertices.Max(point => point.Y);
            var yMin = allVertices.Min(point => point.Y);
            var visibleArea = new RectangleF(
                graphics.VisibleClipBounds.X + paddingLeft,
                graphics.VisibleClipBounds.Y + paddingTop,
                graphics.VisibleClipBounds.Width - paddingRight - paddingLeft,
                graphics.VisibleClipBounds.Height - paddingBottom - paddingTop);
            var coeff = Math.Min(visibleArea.Width, visibleArea.Height);
            foreach (var cityInfo in cityInfos)
            {
                graphics.FillPolygon(
                    new SolidBrush(cityInfo.Color),
                    cityInfo.Vertices.Select(
                        pnt =>
                        new PointF(
                            visibleArea.Left + visibleArea.Width / 2 + coeff * ( ( pnt.X - xMin ) / ( xMax - xMin ) - 0.5f ),
                            visibleArea.Top + visibleArea.Height / 2 - coeff * ( ( pnt.Y - yMin ) / ( yMax - yMin ) - 0.5f )))
                            .ToArray());
            }
        }

        private static void DrawLegend(
            Graphics graphics,
            int paddingLeft,
            int paddingTop,
            IDictionary<Color, string> legendItems,
            string legendCaption,
            Font legendFont,
            Color legendFontColor,
            float lineSpacing)
        {
            graphics.DrawString(legendCaption, legendFont, new SolidBrush(legendFontColor), paddingTop, paddingLeft);
            var captionSize = graphics.MeasureString(legendCaption, legendFont);
            var allColors = legendItems.ToList();
            var allCaptions = legendItems.Values;
            var allCaptionSizes = allCaptions.Select(it => graphics.MeasureString(it, legendFont)).ToList();
            var maxLegendItemWidth = allCaptionSizes.Max(it => it.Width);
            var maxLegendItemHeight = allCaptionSizes.Max(it => it.Height);
            var height = maxLegendItemHeight * lineSpacing;
            for (var i = 0; i < allColors.Count; i++)
            {
                var color = allColors[ i ];
                graphics.DrawString(
                    color.Value,
                    legendFont,
                    new SolidBrush(legendFontColor),
                    paddingLeft,
                    paddingTop + i * height + captionSize.Height * lineSpacing);
                graphics.FillRectangle(
                    new SolidBrush(color.Key),
                    paddingLeft + maxLegendItemWidth + 5,
                    paddingTop + i * height + captionSize.Height * lineSpacing,
                    50,
                    height);
            }
        }

        private static IList<CityInfo> GetCityInfos(geoEntities geoEntitiesContext)
        {
            var cityInfos = new List<CityInfo>();
            foreach (var cityItem in geoEntitiesContext.cities)
            {
                var geometry = cityItem.geometrie_fld;
                if (!geometry.IsValid)
                {
                    continue;
                }
                var cityVertices = new List<PointF>();
                cityInfos.Add(
                    new CityInfo
                        {
                            Vertices = cityVertices,
                            Color = ColorTranslator.FromHtml(cityItem.show_color)
                        });
                for (var i = 1; i <= geometry.PointCount; i++)
                {
                    var point = geometry.PointAt(i);

                    if (point.XCoordinate != null && point.YCoordinate != null)
                    {
                        cityVertices.Add(new PointF((float) point.XCoordinate.Value, (float) point.YCoordinate.Value));
                    }
                }
            }
            return cityInfos;
        }
    }
}