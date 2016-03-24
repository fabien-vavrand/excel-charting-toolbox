using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Toolbox.Geometry.Transformation
{
    public class Homothety
    {
        public Point Center { get; set; }
        public double Coefficient { get; set; }

        public Homothety(Point center, double coefficient)
        {
            Center = center;
            Coefficient = coefficient;
        }

        public Point Transform(Point point)
        {
            return new Point(
                Center.X + Coefficient * (point.X - Center.X),
                Center.Y + Coefficient * (point.Y - Center.Y));
        }

        public Rect Transform(Rect rect)
        {
            Point point = Transform(new Point(rect.X, rect.Y));
            return new Rect(point.X, point.Y, rect.Width * Coefficient, rect.Height * Coefficient);
        }
    }
}
