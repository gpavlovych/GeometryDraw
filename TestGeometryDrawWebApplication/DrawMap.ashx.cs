using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;
using MapVizualizer;

namespace TestGeometryDrawWebApplication
{
    /// <summary>
    /// Summary description for DrawMap
    /// </summary>
    public class DrawMap : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            var legendCaption = "Legenda";
            var legend = new Dictionary<Color, string>()
                             {
                                 { ColorTranslator.FromHtml("#63BE7B"), ">95%" },
                                 { ColorTranslator.FromHtml("#BDD881"), "90-95%" },
                                 { ColorTranslator.FromHtml("#E9E583"), "60-90%" },
                                 { ColorTranslator.FromHtml("#FA8E72"), "30-60%" },
                                 { ColorTranslator.FromHtml("#E15151"), "<30%" },
                                 { ColorTranslator.FromHtml("#BFBFBF"), "Onbekend" }
                             };
            var font = new Font(FontFamily.GenericMonospace, 8, FontStyle.Bold);
            var legendFontColor = Color.Black;
            var widthStr = context.Request[ "width" ] ?? "";
            var heightStr = context.Request[ "height" ] ?? "";

            int width;
            int height;
            if (!int.TryParse(widthStr, out width))
                width = 500;
            if (!int.TryParse(heightStr, out height))
                height = 500;
            using (
                var result = MapHelper.DrawMap(
                    new Size(width, height),
                    50,
                    50,
                    50,
                    50,
                    Color.White,
                    new Pen(Color.Black, 1.0f),
                    legend,
                    legendCaption,
                    font,
                    legendFontColor,
                    1.5f))
            {
                // set MIME type
                context.Response.ContentType = "image/png";

                // write to response stream
                result.Save(context.Response.OutputStream, ImageFormat.Png);
            }

        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}