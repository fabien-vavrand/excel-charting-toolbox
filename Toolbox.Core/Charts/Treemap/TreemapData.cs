using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Toolbox.Charts.Treemap
{
    public class TreemapData
    {
        public List<string> Indexes { get; set; }
        public double Size { get; set; }
        public object Color { get; set; }

        public TreemapData()
        {
            Indexes = new List<string>();
        }

        public TreemapData(List<string> indexes, double size, object color)
        {
            Indexes = indexes;
            Size = size;
            Color = color;
        }

        public double GetColorValue()
        {
            if (Color is double)
                return Convert.ToDouble(Color);
            else
                return 0;
        }
    }

    public class IndexesComparer : IEqualityComparer<List<string>>
    {
        public bool Equals(List<string> x, List<string> y)
        {
            if (x.Count != y.Count)
                return false;

            for (int i = 0; i < x.Count; i++)
                if (x[i] != y[i])
                    return false;

            return true;
        }

        public int GetHashCode(List<string> obj)
        {
            return String.Join("|", obj.ToArray()).GetHashCode();
        }
    }
}
