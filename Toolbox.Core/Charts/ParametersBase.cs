using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Toolbox.Drawing;

namespace Toolbox.Charts
{
    public abstract class ParametersBase
    {
        public bool ShowTitle { get; set; }
        public string Title { get; set; }
        public bool AutoRefresh { get; set; }
        public IColorSelector Color { get; set; }
        public bool ShowLegend { get; set; }
        public string LegendTitle { get; set; }
        public Position LegendPosition { get; set; }
        public LineOptions LegendBorder { get; set; }
        public StringFormater LegendTextFormater { get; set; }
    }
}
