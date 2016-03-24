using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Toolbox.Geometry
{
    public static class GeometryExtensions
    {
        #region Point
        public static Point AddX(this Point point, double x)
        {
            return new Point(point.X + x, point.Y);
        }

        public static Point AddY(this Point point, double y)
        {
            return new Point(point.X, point.Y + y);
        }

        public static Point Translate(this Point point, Vector vector)
        {
            return Vector.Add(vector, point);
        }

        public static double Distance(this Point point1, Point point2)
        {
            return Math.Sqrt(Math.Pow(point2.X - point1.X, 2) + Math.Pow(point2.Y - point1.Y, 2));
        }

        public static Vector VectorTo(this Point point1, Point point2)
        {
            return new Vector(point2.X - point1.X, point2.Y - point1.Y);
        }
        #endregion

        #region Vector
        public static double AngleWith(this Vector vector1, Vector vector2)
        {
            return Vector.AngleBetween(vector1, vector2) * Math.PI / 180;
        }
        #endregion

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
    }
}
