using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Toolbox.Charts.Treemap;
using Toolbox.Controls;
using Toolbox.Drawing;

namespace Toolbox.ViewModel
{
    public class ColorPaletteViewModel : ViewModelBase
    {
        private MyColorPalette colorPalette;
        public MyColorPalette ColorPalette
        {
            get { return colorPalette; }
            set { Set(ref colorPalette, value, broadcast: true); }
        }

        #region List of Values
        public IEnumerable<KeyValuePair<MyColorPalette, string>> ColorPalettes
        {
            get
            {
                return Utils.EnumKeyValues<MyColorPalette>();
            }
        }
        #endregion

        public ColorPaletteViewModel()
        {
            
        }

        public ColorPalette GetColorPalette(IEnumerable<string> values)
        {
            switch (ColorPalette)
            {
                case MyColorPalette.Rainbow:
                    return new ColorPalette(ColorGradient.RainbowPalette()).InitColors(values);
                case MyColorPalette.Spring:
                    return new ColorPalette(ColorGradient.SpringPalette()).InitColors(values);
                default:
                    break;
            }
            return new ColorPalette(ColorGradient.RainbowPalette()).InitColors(values);
        }
    }

    public enum MyColorPalette
    {
        [Description("Rainbow")]
        Rainbow,
        [Description("Spring")]
        Spring
    }
}
