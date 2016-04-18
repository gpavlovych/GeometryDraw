using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using TestGeometryDraw;

namespace MapVizualizer
{
    public class MapHelper
    {
        public static Image DrawMap(
            Size imageSize,
            int paddingLeft,
            int paddingRight,
            int paddingTop,
            int paddingBottom,
            Color backColor,
            IDictionary<Color, string> getLegendItem,
            string legendCaption,
            Font legendFont,
            Color legendFontColor,
            float lineSpacing)
        {

            var result = new Bitmap(imageSize.Width, imageSize.Height, PixelFormat.Format24bppRgb);
            using (var graphics = Graphics.FromImage(result))
            {
                graphics.SmoothingMode = SmoothingMode.AntiAlias;
                graphics.Clear(backColor);
                using (var geoEntitiesContext = new geoEntities())
                {
                    var points = new List<dynamic>();
                    foreach (var cityItem in geoEntitiesContext.cities)
                    {
                        var geometry = cityItem.geometrie_fld;
                        if (!geometry.IsValid)
                        {
                            continue;
                        }
                        var pointsItem = new List<PointF>();
                        points.Add(
                            new
                                {
                                    list = pointsItem,
                                    color = ColorTranslator.FromHtml(cityItem.show_color)
                                });
                        for (var i = 1; i <= geometry.PointCount; i++)
                        {
                            var point = geometry.PointAt(i);

                            if (point.XCoordinate != null && point.YCoordinate != null)
                            {
                                pointsItem.Add(
                                    new PointF((float) point.XCoordinate.Value, (float) point.YCoordinate.Value));

                            }
                        }
                    }
                    var allPoints = points.SelectMany<dynamic, PointF>(pntItem => pntItem.list).ToList();
                    var xMax = allPoints.Max(point => point.X);
                    var xMin = allPoints.Min(point => point.X);
                    var yMax = allPoints.Max(point => point.Y);
                    var yMin = allPoints.Min(point => point.Y);
                    var rect = new RectangleF(
                        graphics.VisibleClipBounds.X + paddingLeft,
                        graphics.VisibleClipBounds.Y + paddingTop,
                        graphics.VisibleClipBounds.Width - paddingRight - paddingLeft,
                        graphics.VisibleClipBounds.Height - paddingBottom - paddingTop);
                    var coeff = Math.Min(rect.Width, rect.Height);
                    foreach (var pointsItem in points)
                    {
                        graphics.FillPolygon(
                            new SolidBrush((Color) pointsItem.color),
                            ( (IEnumerable<PointF>) pointsItem.list ).Select(
                                pnt =>
                                new PointF(
                                    rect.Left + rect.Width / 2 + coeff * ( ( pnt.X - xMin ) / ( xMax - xMin ) - 0.5f ),
                                    rect.Top + rect.Height / 2 - coeff * ( ( pnt.Y - yMin ) / ( yMax - yMin ) - 0.5f )))
                                                                     .ToArray());
                    }
                    graphics.DrawString(
                        legendCaption,
                        legendFont,
                        new SolidBrush(legendFontColor),
                        paddingTop,
                        paddingLeft);
                    var captionSize = graphics.MeasureString(legendCaption, legendFont);
                    var allColors = getLegendItem.ToList();
                    var allCaptions = getLegendItem.Values;
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
            }
            return result;
        }
    }
}