using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using Toolbox.Drawing;
using System.Drawing;
using System;

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
        public double LegendSize { get; set; }
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

        public void BuildLegend(IColorSelector color, Position position, StringFormater formater)
        {
            LegendSize = 10;
            float maxWidth = 0f;
            List<string> legendTexts = new List<string>();

            if (color is ColorGradient)
            {
                ColorGradient gradient = color as ColorGradient;
                legendTexts = gradient.Stops.Select(s => formater.Format(s.Value)).ToList();
            }
            else if (color is ColorPalette)
            {
                ColorPalette palette = color as ColorPalette;
                legendTexts = palette.Colors.Keys.Select(s => formater.Format(s)).ToList();
            }

            maxWidth = legendTexts
                    .Max(s => s.ToString().TextWidth(new Font("Calibri", 10)));
            LegendTextHeight = legendTexts
                .Max(s => s.ToString().TextHeight(new Font("Calibri", 10)));

            switch (position)
            {
                case Position.Left:
                case Position.Right:
                    PlotArea = PlotArea.WithWidth(PlotArea.Width - LegendSize - Margin - maxWidth);
                    LegendArea = new Rect(
                        PlotArea.Left + PlotArea.Width + Margin,
                        PlotArea.Top,
                        LegendSize,
                        PlotArea.Height);
                    break;

                case Position.Top:
                case Position.Bottom:
                    PlotArea = PlotArea.WithHeight(PlotArea.Height - LegendSize - Margin - LegendTextHeight);
                    LegendArea = new Rect(
                        PlotArea.Left,
                        PlotArea.Top + PlotArea.Height + Margin,
                        PlotArea.Width,
                        LegendSize);
                    break;

                default:
                    break;
            }
        }

        public Excel.Shape PrintLegendText(double left, double top, object text, StringFormater formater)
        {
            Excel.Shape legend = Chart.Shapes.AddShape(
                                        Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle,
                                        (float)left, (float)top, 1f, 1f);

            legend.Line.Visible = GetState(false);
            legend.Fill.ForeColor.RGB = Color.White.ToRgb();
            legend.Fill.Transparency = 1f;
            legend.TextFrame.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            legend.TextFrame.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            legend.TextFrame.Characters().Text = formater.Format(text);
            legend.TextFrame.Characters().Font.Name = "Calibri";
            legend.TextFrame.Characters().Font.Size = 10;
            legend.TextFrame.Characters().Font.Color = Color.Black.ToRgb();
            legend.TextFrame.AutoSize = true;

            legend.Left = (float)left;
            legend.Top = (float)top;
            return legend;
        }

        public Microsoft.Office.Core.MsoTriState GetState(bool value)
        {
            if (value)
                return Microsoft.Office.Core.MsoTriState.msoTrue;
            else
                return Microsoft.Office.Core.MsoTriState.msoFalse;
        }
    }
}
