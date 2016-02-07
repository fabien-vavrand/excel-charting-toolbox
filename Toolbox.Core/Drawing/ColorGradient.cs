using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Toolbox.Charts;

namespace Toolbox.Drawing
{
    public class ColorGradient : IColorSelector
    {
        public List<ColorGradientStop> Stops { get; set; }

        public ColorGradient()
        {
            Stops = new List<ColorGradientStop>();
        }

        public ColorGradient AddStop(double value, Color color)
        {
            Stops.Add(new ColorGradientStop(value, color));
            Stops = Stops.OrderBy(s => s.Value).ToList();
            return this;
        }

        public ColorGradient AddStop(double value, int r, int g, int b)
        {
            AddStop(value, 255, r, g, b);
            return this;
        }

        public ColorGradient AddStop(double value, int a, int r, int g, int b)
        {
            AddStop(value, Color.FromArgb(a, r, g, b));
            return this;
        }

        public Color GetColor(double value)
        {
            double v = value.CapFloor(Stops.First().Value, Stops.Last().Value);
            int index = Stops.FindLastIndex(s => s.Value <= v);

            if (v == Stops.Last().Value)
                return Stops.Last().Color;

            ColorGradientStop s0 = Stops[index];
            ColorGradientStop s1 = Stops[index + 1];

            return GetGradient(s0.Color, s1.Color, (v - s0.Value) / (s1.Value - s0.Value));
        }

        private Color GetGradient(Color color1, Color color2, double ratio)
        {
            return Color.FromArgb(
                (int)Math.Round(color1.A * (1 - ratio) + ratio * color2.A, 0),
                (int)Math.Round(color1.R * (1 - ratio) + ratio * color2.R, 0),
                (int)Math.Round(color1.G * (1 - ratio) + ratio * color2.G, 0),
                (int)Math.Round(color1.B * (1 - ratio) + ratio * color2.B, 0));
        }

        public Color GetColor(object value)
        {
            return GetColor(value.ToDouble());
        }

        public static ColorGradient RainbowPalette() 
        {
            double n = 6;
            return new ColorGradient()
                .AddStop(0d / n, Color.Red)
                .AddStop(1d / n, Color.Orange)
                .AddStop(2d / n, Color.Yellow)
                .AddStop(3d / n, Color.Green)
                .AddStop(4d / n, Color.Blue)
                .AddStop(5d / n, Color.Indigo)
                .AddStop(6d / n, Color.Violet);
        }

        public static ColorGradient SpringPalette()
        {
            double n = 4;
            return new ColorGradient()
                .AddStop(0d / n, 161, 221, 115)
                .AddStop(1d / n, 55, 161, 68)
                .AddStop(2d / n, 25, 144, 87)
                .AddStop(3d / n, 0, 111, 86)
                .AddStop(4d / n, 12, 96, 90);
        }
    }

    public class ColorGradientStop
    {
        public double Value { get; set; }
        public Color Color { get; set; }

        public ColorGradientStop(double value, Color color)
        {
            Value = value;
            Color = color;
        }
    }
}
