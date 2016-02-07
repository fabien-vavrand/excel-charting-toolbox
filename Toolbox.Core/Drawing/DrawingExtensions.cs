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
    }
}
