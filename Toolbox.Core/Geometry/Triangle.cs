using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Geometry
{
    public struct Triangle
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }
        public Point Point3 { get; set; }

        public Triangle(double x1, double y1, double x2, double y2, double x3, double y3) : this()
        {
            Point1 = new Point(x1, y1);
            Point2 = new Point(x2, y2);
            Point3 = new Point(x3, y3);
        }

        public Triangle(Point p1, Point p2, Point p3) : this()
        {
            Point1 = p1;
            Point2 = p2;
            Point3 = p3;
        }

        public Point Center()
        {
            return new Point((Point1.X + Point2.X + Point3.X) / 3, (Point1.Y + Point2.Y + Point3.Y) / 3);
        }

        public static double AreaByHeight(double basis, double height)
        {
            return basis * height / 2;
        }

        public static double AreaByLenghts(double a, double b, double c)
        {
            double p = (a + b + c) / 2;
            return Math.Sqrt(p * (p - a) * (p - b) * (p - c));
        }

        public static double AreaByAngle(double a, double b, double gamma)
        {
            return 1 / 2 * a * b * Math.Sin(gamma);
        }

        public static Triangle FromCenterAndLenghts(Point center, double l1, double l2, double l3)
        {
            Point p1 = new Point(0, 0);
            Point p2 = new Point(0, l1);
            double area = AreaByLenghts(l1, l2, l3);
            double height = area * 2 / l1;
            double l1bis = Math.Sqrt(Math.Pow(l3, 2) - Math.Pow(height, 2));
            Point p3 = new Point(height, l1bis);
            Point g = new Triangle(p1, p2, p3).Center();
            Vector translation = new Vector(center.X - g.X, center.Y - g.Y);
            return new Triangle(
                p1.Translate(translation),
                p2.Translate(translation),
                p3.Translate(translation));
        }

        public static Triangle FromTwoPointsAndTwoLengths(Point point1, Point point2, double l1, double l2)
        {
            double l3 = point1.Distance(point2);
            double area = Triangle.AreaByLenghts(l1, l2, l3);
            double angle = Math.Asin(2 * area / (l1 * l3));
            Vector horizontal = point1.VectorTo(point1.AddX(1));
            double angle0 = horizontal.AngleWith(point1.VectorTo(point2));
            double x = l1 * Math.Cos(angle0 - angle);
            double y = l1 * Math.Sin(angle0 - angle);
            Point point3 = point1.AddX(x).AddY(y);
            return new Triangle(point1, point2, point3);
        }
    }
}
