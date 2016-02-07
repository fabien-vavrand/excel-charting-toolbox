using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Toolbox.Drawing;
using Toolbox;

namespace Toolbox.Charts
{
    public interface IColorSelector
    {
        Color GetColor(object value);
    }

    //public class TreemapColorGradient : IColorSelector
    //{
    //    public ColorGradient Gradient { get; set; }

    //    public TreemapColorGradient()
    //    {
    //        Gradient = new ColorGradient();
    //    }

    //    public TreemapColorGradient(double value, int r, int g, int b) : this()
    //    {
    //        AddColor(value, r, g, b);
    //    }

    //    public TreemapColorGradient(double value, int a, int r, int g, int b)
    //        : this()
    //    {
    //        AddColor(value, a, r, g, b);
    //    }

    //    public TreemapColorGradient AddColor(double value, int r, int g, int b)
    //    {
    //        Gradient.AddStop(value, r, g, b);
    //        return this;
    //    }

    //    public TreemapColorGradient AddColor(double value, int a, int r, int g, int b)
    //    {
    //        Gradient.AddStop(value, a, r, g, b);
    //        return this;
    //    }

    //    public Color GetColor(object value)
    //    {
    //        return Gradient.GetColor(value.ToDouble());
    //    }
    //}
}
