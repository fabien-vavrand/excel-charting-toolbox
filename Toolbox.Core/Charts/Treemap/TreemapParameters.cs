using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox.Drawing;

namespace Toolbox.Charts.Treemap
{
    public class TreemapParameters : ParametersBase
    {
        public TreemapAlgorithm Algorithm { get; set; }
        public List<TreemapIndex> Indexes { get; set; }

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
