using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

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

        #region Text
        //Try TextRenderer in .NET 4.5
        public static float TextWidth(this string text, Font f)
        {
            float textWidth = 0;

            using (Bitmap bmp = new Bitmap(1, 1))
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                textWidth = g.MeasureString(text, f).Width;
            }

            return textWidth;
        }

        public static float TextHeight(this string text, Font f)
        {
            float textHeight = 0;

            using (Bitmap bmp = new Bitmap(1, 1))
            using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(bmp))
            {
                textHeight = g.MeasureString(text, f).Height;
            }

            return textHeight;
        }
        #endregion
    }
}
