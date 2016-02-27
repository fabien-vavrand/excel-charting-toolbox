using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using Toolbox.Drawing;
using System.Drawing;

namespace Toolbox.Charts
{
    public abstract class ChartBase
    {  
        public Excel.Chart Chart { get; set; }
        public List<Excel.Shape> Shapes { get; set; }
        public bool IsActive { get; set; }

        public Rect ChartArea { get; set; }
        public Rect PlotArea { get; set; }
        public Rect TitleArea { get; set; }
        public Rect LegendArea { get; set; }

        public double Margin { get; set; }
        public double Offset { get; set; }
        public double LegendTextHeight { get; set; }

        public void BuildArea()
        {
            PlotArea = new Rect(
                Margin,
                Margin,
                ChartArea.Width - 2 * Margin - Offset,
                ChartArea.Height - 2 * Margin - Offset);
        }

        public void BuildTitle()
        {
            TitleArea = new Rect(
                PlotArea.Left,
                0,
                PlotArea.Width,
                30);
            PlotArea = PlotArea
                .WithTop(PlotArea.Top + TitleArea.Height)
                .WithHeight(PlotArea.Height - TitleArea.Height);
        }

        public void BuildLegend(IColorSelector color, Position position)
        {
            if (color is ColorGradient)
            {
                double legendSize = 10;
                float maxWidth = 0f;

                ColorGradient gradient = color as ColorGradient;
                maxWidth = gradient.Stops
                    .Max(s => s.Value.ToString().TextWidth(new Font("Calibri", 10)));
                LegendTextHeight = gradient.Stops
                    .Max(s => s.Value.ToString().TextHeight(new Font("Calibri", 10)));

                switch (position)
                {
                    case Position.Left:
                    case Position.Right:
                        PlotArea = PlotArea.WithWidth(PlotArea.Width - legendSize - Margin - maxWidth);
                        LegendArea = new Rect(
                            PlotArea.Left + PlotArea.Width + Margin,
                            PlotArea.Top,
                            legendSize,
                            PlotArea.Height);
                        break;

                    case Position.Top:
                    case Position.Bottom:
                        PlotArea = PlotArea.WithHeight(PlotArea.Height - legendSize - Margin - LegendTextHeight);
                        LegendArea = new Rect(
                            PlotArea.Left,
                            PlotArea.Top + PlotArea.Height + Margin,
                            PlotArea.Width,
                            legendSize);
                        break;

                    default:
                        break;
                }
            }
        }
    }
}
