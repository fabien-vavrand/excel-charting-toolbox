using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Drawing
{
    public class Rectangle
    {
        public double Top { get; set; }
        public double Left { get; set; }
        public double Height { get; set; }
        public double Width { get; set; }

        public Rectangle(double height, double width)
        {
            Height = height;
            Width = width;
        }

        public Rectangle(double top, double left, double height, double width)
        {
            Top = top;
            Left = left;
            Height = height;
            Width = width;
        }

        public double Area()
        {
            return Height * Width;
        }

        public double AspectRatio()
        {
            return Math.Max(Width, Height) / Math.Min(Width, Height);
        }

        public bool IsHorizontal()
        {
            return Height < Width;
        }

        public bool IsVertical()
        {
            return Height > Width;
        }

        public bool IsDegenerated()
        {
            return Double.IsNaN(Height) || Double.IsNaN(Width);
        }

        public void AddMargin(double margin)
        {
            Top += margin;
            Left += margin;
            Height -= 2 * margin;
            Width -= 2 * margin;

            Height = Math.Max(Height, 0);
            Width = Math.Max(Width, 0);
        }

        public void AddMargin(Margin margin)
        {
            AddMargin(margin.Left, margin.Top, margin.Right, margin.Bottom);
        }

        public void AddMargin(double left, double top, double right, double bottom)
        {
            Top += top;
            Left += left;
            Height -= top + bottom;
            Width -= left + right;

            Height = Math.Max(Height, 0);
            Width = Math.Max(Width, 0);
        }
    }
}
