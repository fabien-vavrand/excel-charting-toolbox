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

        public bool ShowLegend { get; set; }
        public Position LegendPosition { get; set; }

        public TreemapParameters()
        {
            Indexes = new List<TreemapIndex>();
            ShowLegend = true;
            LegendPosition = Position.Bottom;
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
}
