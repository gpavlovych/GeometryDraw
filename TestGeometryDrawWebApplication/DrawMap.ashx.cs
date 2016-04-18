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
            using (
                var result = MapHelper.DrawMap(
                    new Size(500, 500),
                    50,
                    50,
                    50,
                    50,
                    Color.White,
                    legend,
                    legendCaption,
                    font,
                    legendFontColor,
                    1.5f))
            {
                // set MIME type
                context.Response.ContentType = "image/jpeg";

                // write to response stream
                result.Save(context.Response.OutputStream, ImageFormat.Jpeg);
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