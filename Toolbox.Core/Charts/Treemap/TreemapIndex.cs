using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using Toolbox.Drawing;
using Toolbox.Geometry;

namespace Toolbox.Charts.Treemap
{
    public class TreemapIndex
    {
        public bool HasHeader { get; set; }
        public Margin Padding { get; set; }
        public Color FillColor { get; set; }

        public bool LineVisible { get; set; }
        public double LineWeight { get; set; }
        public Color LineColor { get; set; }

        public double FontSize { get; set; }
        public Color FontColor { get; set; }
        public bool FontBold { get; set; }

        public bool FontOutline { get; set; }
        public Color FontOutlineColor { get; set; }
        public double FontOutlineWeight { get; set; }

        public double FontGlowRadius { get; set; }
        public Color FontGlowColor { get; set; }

        public LineOptions GetLineOptions()
        {
            return new LineOptions()
            {
                Visible = LineVisible, 
                Weight = LineWeight,
                Color = LineColor
            };
        }

        public TreemapIndex()
        {
            HasHeader = false;
            Padding = new Margin(0);
            FillColor = Color.Transparent;
            LineVisible = true;
            LineWeight = 1f;
            LineColor = Color.White;
            FontSize = 11;
            FontColor = Color.Black;
            FontBold = false;
            FontOutline = false;
            FontOutlineColor = Color.Black;
            FontOutlineWeight = 1;
            FontGlowRadius = 0;
            FontGlowColor = Color.Transparent;
        }

        public static TreemapIndex FirstIndex = new TreemapIndex()
        {
            LineWeight = 2f,
            LineColor = Color.Black,
            FontSize = 26,
            FontColor = Color.White,
            FontBold = true,
            FontOutline = true,
            FontGlowRadius = 10,
            FontGlowColor = Color.FromArgb(230, 240, 240, 240)
        };

        public static TreemapIndex LastIndex = new TreemapIndex();
    }
}
