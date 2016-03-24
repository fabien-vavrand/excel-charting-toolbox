using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;

namespace Toolbox.Geometry.Transformation
{
    public class Translation
    {
        public Vector Vector { get; set; }

        public Translation(Vector vector)
        {
            Vector = vector;
        }

        public Point Translate(Point point)
        {
            return Vector.Add(Vector, point);
        }

        public Rect Translate(Rect rect)
        {
            return new Rect(rect.Left + Vector.X, rect.Top + Vector.Y, rect.Width, rect.Height);
        }
    }
}
