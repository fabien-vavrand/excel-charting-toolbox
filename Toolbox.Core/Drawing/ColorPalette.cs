using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Toolbox.Charts;

namespace Toolbox.Drawing
{
    public class ColorPalette : IColorSelector
    {
        public ColorGradient Gradient { get; set; }
        public Dictionary<string, Color> Colors { get; set; }

        public ColorPalette(ColorGradient gradient)
        {
            Gradient = gradient;
        }

        public ColorPalette InitColors(IEnumerable<string> values)
        {
            Colors = new Dictionary<string, Color>();
            double min = Gradient.Stops.First().Value;
            double max = Gradient.Stops.Last().Value;
            var stops = values.Distinct().OrderBy(v => v).ToList();

            if (stops.Count == 1)
            {
                Colors.Add(stops.First(), Gradient.GetColor(min));
                return this;
            }

            for (int i = 0; i < stops.Count; i++)
            {
                double ratio = (double)i / ((double)stops.Count - 1);
                double value = min + (max - min) * ratio;
                Colors.Add(stops[i], Gradient.GetColor(value));
            }  
            return this;
        }

        public Color GetColor(object value)
        {
            Color color;
            if (Colors.TryGetValue((value ?? String.Empty).ToString(), out color))
                return color;
            else
                return Color.White;
        }
    }
}
