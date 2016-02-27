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
        #region Rectangle
        public static bool IsHorizontal(this Rect rectangle)
        {
            return rectangle.Height < rectangle.Width;
        }

        public static bool IsVertical(this Rect rectangle)
        {
            return rectangle.Height > rectangle.Width;
        }

        public static double Area(this Rect rectangle)
        {
            if (Double.IsNaN(rectangle.Height) || Double.IsNaN(rectangle.Width))
                return 0;

            return rectangle.Height * rectangle.Width;
        }

        public static double AspectRatio(this Rect rectangle)
        {
            return Math.Max(rectangle.Width, rectangle.Height) / Math.Min(rectangle.Width, rectangle.Height);
        }

        public static bool IsDegenerated(this Rect rectangle)
        {
            return rectangle.Area() == 0;
        }

        public static Rect ApplyMargins(this Rect rect, Margin margin)
        {
            return new Rect(
                rect.Left + margin.Left,
                rect.Top + margin.Top,
                rect.Width - margin.Left - margin.Right,
                rect.Height - margin.Top - margin.Bottom);
        }

        public static Rect WithLeft(this Rect rect, double left)
        {
            return new Rect(
                left,
                rect.Top,
                rect.Width,
                rect.Height);
        }

        public static Rect WithTop(this Rect rect, double top)
        {
            return new Rect(
                rect.Left,
                top,
                rect.Width,
                rect.Height);
        }

        public static Rect WithWidth(this Rect rect, double width)
        {
            return new Rect(
                rect.Left,
                rect.Top,
                width,
                rect.Height);
        }

        public static Rect WithHeight(this Rect rect, double height)
        {
            return new Rect(
                rect.Left,
                rect.Top,
                rect.Width,
                height);
        }
        #endregion

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
