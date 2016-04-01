using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Toolbox.Drawing
{
    public static class DrawingExtensions
    {
        #region Color
        public static float GetAlpha(this Color color)
        {
            return 1 - (float)color.A / (float)Byte.MaxValue;
        }

        public static int ToRgb(this Color color)
        {
            return (65536 * color.B) + (256 * color.G) + (color.R);
        }
        #endregion

        #region Font
        public static SizeF RenderText(this Font font, string text)
        {
            using (Bitmap bmp = new Bitmap(1, 1))
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
                return g.MeasureString(text, font, new Point(0, 0), StringFormat.GenericTypographic);
           
            //return (float)TextRenderer.MeasureText(text, font, System.Drawing.Size.Empty, TextFormatFlags.NoPadding);
        }
        #endregion
    }
}
