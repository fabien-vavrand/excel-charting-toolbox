using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Drawing
{
    public class Margin
    {
        public double Left { get; set; }
        public double Top { get; set; }
        public double Right { get; set; }
        public double Bottom { get; set; }

        public Margin(double margin)
        {
            Left = margin;
            Top = margin;
            Right = margin;
            Bottom = margin;
        }

        public Margin(double leftright, double topbottom)
        {
            Left = leftright;
            Top = topbottom;
            Right = leftright;
            Bottom = topbottom;
        }

        public Margin(double left, double top, double right, double bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
