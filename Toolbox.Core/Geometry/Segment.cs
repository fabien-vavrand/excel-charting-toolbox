using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Geometry
{
    public struct Segment
    {
        public Point Point1 { get; set; }
        public Point Point2 { get; set; }

        public Segment(Point point1, Point point2) : this()
        {
            Point1 = point1;
            Point2 = point2;
        }

        public Point Middle()
        {
            return new Point((Point1.X + Point2.X) / 2, (Point1.Y + Point2.Y) / 2);
        }

        public Vector ToVector()
        {
            return new Vector(Point2.X - Point1.X, Point2.Y - Point1.Y);
        }
    }
}
