using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox.Drawing;

namespace Toolbox.Charts.Treemap
{
    public class TreemapParameters
    {
        public List<TreemapIndex> Indexes { get; set; }
        public IColorSelector Color { get; set; }
        public TreemapAlgorithm Algorithm { get; set; }

        public bool ShowLegend { get; set; }
        public Position LegendPosition { get; set; }
        public StringFormater LegendTextFormater { get; set; }

        public bool ShowTitle { get; set; }
        public string Title { get; set; }


        public TreemapParameters()
        {
            Indexes = new List<TreemapIndex>();
            ShowLegend = true;
            LegendPosition = Position.Bottom;
            LegendTextFormater = new StringFormater();
        }

        public TreemapParameters AddIndex(TreemapIndex index) 
        {
            Indexes.Add(index);
            return this;
        }

        public TreemapParameters WithColor(IColorSelector color)
        {
            Color = color;
            return this;
        }
    }

    public enum TreemapAlgorithm
    {
        Squarify,
        Circular
    }
}
