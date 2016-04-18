using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Spatial;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Linq;
using System.Windows.Forms;
using MapVizualizer;

namespace TestGeometryDraw
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region Overrides of Form

        /// <param name="e">A <see cref="T:System.Windows.Forms.PaintEventArgs"/> that contains the event data. </param>
        protected override void OnPaint(PaintEventArgs e)
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
            e.Graphics.Clear(Color.White);
            using (
                var result = MapHelper.DrawMap(
                    new Size(e.ClipRectangle.Width, e.ClipRectangle.Height),
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
                e.Graphics.DrawImageUnscaled(result, e.ClipRectangle.Left, e.ClipRectangle.Top);
            }
            base.OnPaint(e);
        }

        #endregion
    }
}
