using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Toolbox.Charts.Treemap;
using Toolbox.Controls;
using Toolbox.Drawing;

namespace Toolbox.ViewModel
{
    public class Gradient2ColorsViewModel : ViewModelBase
    {
        #region Properties
        private Color lowColor;
        public Color LowColor
        {
            get { return lowColor; }
            set { Set(ref lowColor, value, broadcast: true); }
        }

        private Color highColor;
        public Color HighColor
        {
            get { return highColor; }
            set { Set(ref highColor, value, broadcast: true); }
        }

        public Wrapper<double> LowValue { get; set; }
        public Wrapper<double> HighValue { get; set; }
        #endregion

        public Gradient2ColorsViewModel()
        {
            Func<object, Tuple<bool, double>> converter = (o) => Tuple.Create(o.IsDouble(), o.ToDouble());
            LowValue = new Wrapper<double>(converter);
            HighValue = new Wrapper<double>(converter);
        }

        public Gradient2ColorsViewModel InitValues(List<double> values)
        {
            double min = Math.Floor(values.Percentile(0.05));
            double max = Math.Ceiling(values.Percentile(0.95));

            if (min == max)
                max += 1;

            LowValue.Value = min;
            HighValue.Value = max;

            return this;
        }

        public ColorGradient GetColorGradient()
        {
            ColorGradient gradient = new ColorGradient()
                .AddStop(LowValue.Value, LowColor.R, LowColor.G, LowColor.B)
                .AddStop(HighValue.Value, HighColor.R, HighColor.G, HighColor.B);
            return gradient;
        }
    }
}
