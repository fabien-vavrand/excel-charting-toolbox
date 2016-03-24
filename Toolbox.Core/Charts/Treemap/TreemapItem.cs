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

        #region Algorithm Selection
        public void ApplyAlgorithm(List<TreemapData> data, TreemapAlgorithm algorithm)
        {
            switch (algorithm)
            {
                case TreemapAlgorithm.Squarify:
                    Squarify(data);
                    break;
                case TreemapAlgorithm.Circular:
                    Circlify(data);
                    break;
            }
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
        
        #region Circlify
        public void Circlify(List<TreemapData> data)
        {
        	//IL faut commencer par les 2 plus gros cercles, le milieu du cercle final étant le centre du segment reliant les extrémités de ces cercles.
        	double a1 = GetInnerCircleArea() * data[0].Size / Size;
        	double a2 = GetInnerCircleArea() * data[1].Size / Size;
        	double r1 = Math.Sqrt(a1 / Math.PI);
        	double r2 = Math.Sqrt(a2 / Math.PI);
        
        	var items = new Dictionary<TreemapItem, List<TreemapItem>>();
        
        	Geometric.Point center = GetCenter();
        	TreemapItem item1 = AddItem(data[0], center.AddX(-r2), r1);
        	TreemapItem item2 = AddItem(data[1], center.AddX(r1), r2);
        	items.Add(item1, new List<TreemapItem> { item2 });
        	items.Add(item2, new List<TreemapItem> { item1 });
        
        	for (int d = 2; d < data.Count; d++)
        	{
        		double a = GetInnerCircleArea() * data[d].Size / Size;
        		double r = Math.Sqrt(a / Math.PI);
        		var results = new List<Tuple<TreemapItem, TreemapItem, Geometric.Point, double>>();
        
        		foreach (var kvp in items)
        		{
        			TreemapItem i1 = kvp.Key;
        			foreach (TreemapItem i2 in kvp.Value)
        			{
        				Triangle t = Triangle.FromTwoPointsAndTwoLengths(i1.GetCenter(), i2.GetCenter(),
        					i1.GetRadius() + r, i2.GetRadius() + r);
        				double distance = center.Distance(t.Point3);
        
        				if (Items.All(item => item.GetCenter().Distance(t.Point3) - (item.GetRadius() + r) >= -0.0001))
        					results.Add(Tuple.Create(i1, i2, t.Point3, distance));
        			}
        		}
        
        		var result = results.OrderBy(t => t.Item4).First();
        		TreemapItem newItem = AddItem(data[d], result.Item3, r);
        		items[result.Item1].Add(newItem);
        		items[result.Item2].Add(newItem);
        		items.Add(newItem, new List<TreemapItem> { result.Item1, result.Item2 });
        	}
        
        	//Identify the item farther from the center and expand the chart so that this item becomes adjacent to external circle
        	var max = Items
        		.Select(i => new { Item = i, Distance = i.GetCenter().Distance(center) + i.GetRadius() })
        		.OrderByDescending(o => o.Distance)
        		.First();
        
        	Homothety homothety = new Homothety(center, GetRadius() / max.Distance);
        	foreach (var item in Items)
        	{
        		item.Rectangle = homothety.Transform(item.Rectangle);
        		item.InnerRectangle = homothety.Transform(item.InnerRectangle);
        		item.Empty = homothety.Transform(item.Empty);
        	}
        
        	//Homothety from contact point
        	Vector vector = new Segment(center, max.Item.GetCenter()).ToVector();
        	Geometric.Point contact = max.Item.GetCenter()
        		.AddX(max.Item.GetRadius() / vector.Length * vector.X)
        		.AddY(max.Item.GetRadius() / vector.Length * vector.Y);
        
        	NewtonSolver solver = new NewtonSolver((c) =>
        	{
        		Homothety h = new Homothety(contact, c);
        		double min = Items
        			.Except(new List<TreemapItem> { max.Item })
        			.Select(i => h.Transform(i.InnerRectangle))
        			.Select(r => new
        			{
        				Center = new Geometric.Point(r.Left + r.Width / 2, r.Top + r.Height / 2),
        				Radius = r.Width / 2
        			})
        			.Min(r => GetRadius() - (r.Center.Distance(center) + r.Radius));
        
        		return min;
        	})
        	.Solve(1);
        
        	homothety = new Homothety(contact, solver.Solution);
        	foreach (var item in Items)
        	{
        		item.Rectangle = homothety.Transform(item.Rectangle);
        		item.InnerRectangle = homothety.Transform(item.InnerRectangle);
        		item.Empty = homothety.Transform(item.Empty);
        	}
        }
        
        public double GetRadius()
        {
        	return InnerRectangle.Width / 2;
        }
        
        public double GetInnerCircleArea()
        {
        	return Math.PI * Math.Pow(InnerRectangle.Width / 2, 2);
        }
        
        public Geometric.Point GetCenter()
        {
        	return new Geometric.Point(
        		InnerRectangle.Left + InnerRectangle.Width / 2,
        		InnerRectangle.Top + InnerRectangle.Height / 2);
        }
        
        public TreemapItem AddItem(TreemapData data, Geometric.Point center, double radius)
        {
        	TreemapItem item = new TreemapItem(center.X - radius, center.Y - radius, 2 * radius, 2 * radius)
        	{
        		Indexes = data.Indexes,
        		Size = data.Size,
        		Color = data.Color
        	};
        
        	Items.Add(item);
        	return item;
        }
        #endregion
    }
}
