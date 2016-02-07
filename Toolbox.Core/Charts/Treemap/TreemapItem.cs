using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows;
using Toolbox.Drawing;
using Excel = Microsoft.Office.Interop.Excel;

namespace Toolbox.Charts.Treemap
{
    public class TreemapItem
    {
        #region Properties
        public List<string> Indexes { get; set; }
        public double Size { get; set; }
        public object Color { get; set; }
        public TreemapIndex IndexParameters { get; set; }
        public Color FillColor { get; set; }
        public Rect Rectangle { get; set; }
        public Rect InnerRectangle { get; set; }
        public Rect Empty { get; set; }
        public List<TreemapItem> Items { get; set; }
        #endregion

        #region Ctor
        public TreemapItem(double left, double top, double width, double height)
        {
            Indexes = new List<string>();
            Rectangle = new Rect(left, top, width, height);
            InnerRectangle = new Rect(left, top, width, height);
            Empty = new Rect(left, top, width, height);

            Items = new List<TreemapItem>();
        }
        #endregion

        #region Methods
        public void SetMargin(double left, double top, double right, double bottom)
        {
            SetMargin(new Margin(left, top, right, bottom));
        }

        public void SetMargin(Margin margin)
        {
            InnerRectangle = InnerRectangle.ApplyMargins(margin);
            Empty = Empty.ApplyMargins(margin);
        }
        #endregion

        #region Squarify
        public void Squarify(List<TreemapData> data)
        {
            for (int i = 0; i < data.Count; i++)
            {
                int n = 1;
                double aspectRatio = AspectRatio(data.GetRange(i, n));
                double nextAspectRatio = aspectRatio;

                while (nextAspectRatio <= aspectRatio && !Empty.IsDegenerated())
                {
                    n++;

                    if (data.Count < i + n)
                        break;

                    aspectRatio = nextAspectRatio;
                    nextAspectRatio = AspectRatio(data.GetRange(i, n));
                }

                n--;
                AddItems(data.GetRange(i, n));
                i += n - 1;

                if (Empty.IsDegenerated())
                    break;
            }
        }

        public double AspectRatio(List<TreemapData> items)
        {
            double area = Area(items.Last().Size);
            if (Empty.IsHorizontal())
            {
                double width = Area(items) / Empty.Height;
                return new Rect(0, 0, area / width, width).AspectRatio();
            }
            else
            {
                double height = Area(items) / Empty.Width;
                return new Rect(0, 0, height, area / height).AspectRatio();
            }
        }

        public void AddItems(List<TreemapData> items)
        {
            double top = Empty.Top;
            double left = Empty.Left;
            if (Empty.IsHorizontal())
            {
                double width = Area(items) / Empty.Height;
                foreach (TreemapData data in items)
                {
                    double area = Area(data.Size);
                    Items.Add(new TreemapItem(left, top, width, area / width)
                    {
                        Indexes = data.Indexes,
                        Size = data.Size,
                        Color = data.Color
                    });
                    top += area / width;
                }
                Empty = new Rect(Empty.Left + width, Empty.Top, (Empty.Width - width).Floor(0), Empty.Height);
            }
            else
            {
                double height = Area(items) / Empty.Width;
                foreach (TreemapData data in items)
                {
                    double area = Area(data.Size);
                    Items.Add(new TreemapItem(left, top, area / height, height)
                    {
                        Indexes = data.Indexes,
                        Size = data.Size,
                        Color = data.Color
                    });
                    left += area / height;
                }
                Empty = new Rect(Empty.Left, Empty.Top + height, Empty.Width, (Empty.Height - height).Floor(0));
            }
        }

        private double Area(List<TreemapData> items)
        {
            return Area(items.Sum(i => i.Size));
        }

        private double Area(double size)
        {
            return InnerRectangle.Area() * size / Size;
        }
        #endregion
    }
}
