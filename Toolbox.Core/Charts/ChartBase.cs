using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using Toolbox.Drawing;
using System.Drawing;
using System;
using Toolbox.Geometry;

namespace Toolbox.Charts
{
    public abstract class ChartBase
    {
        #region Constants
        public const string DefaultFontFamily = "Calibri";
        public const float DefaultFontSize = 10;

        public const double Margin = 8;
        public const double SmallMargin = 4;
        public const double TinyMargin = 2;
        public const double Offset = 3;
        public const double LegendThickness = 10;
        #endregion

        #region Properties
        public Excel.Chart Chart { get; set; }
        public List<Excel.Shape> Shapes { get; set; }
        public bool IsActive { get; set; }

        public Rect ChartArea { get; set; }
        public Rect PlotArea { get; set; }
        public Rect TitleArea { get; set; }
        public Rect LegendTitleArea { get; set; }
        public Rect LegendArea { get; set; }

        public string LegendTitle { get; set; }
        public double LegendTitleWidth { get; set; }
        public double LegendTitleHeight { get; set; }
        public double LegendTextHeight { get; set; }

        public Font DefaultFont { get; set; }
        public Font DefaultFontBold { get; set; }
        #endregion

        #region Ctor
        public ChartBase()
        {
            DefaultFont = new Font(DefaultFontFamily, DefaultFontSize, FontStyle.Regular, GraphicsUnit.Pixel);
            DefaultFontBold = new Font(DefaultFontFamily, DefaultFontSize, FontStyle.Bold, GraphicsUnit.Pixel);
        }
        #endregion

        #region Builders
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

        public void BuildLegend(string title, IColorSelector color, Position position, StringFormater formater)
        {
            LegendTitle = title;
            if (String.IsNullOrEmpty(LegendTitle))
                LegendTitle = "Color";
            if (LegendTitle.Length > 15)
                LegendTitle = LegendTitle.Substring(0, 13) + "...";

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

            var sizes = legendTexts.Select(t => DefaultFont.RenderText(t));
            maxWidth = sizes.Max(s => s.Width);
            LegendTextHeight = sizes.Max(s => s.Height);

            //Legend title
            var titleSize = DefaultFontBold.RenderText(LegendTitle);
            LegendTitleWidth = titleSize.Width;
            LegendTitleHeight = titleSize.Height;

            if (position == Position.Right || position == Position.Left)
                maxWidth = Math.Max(maxWidth, (float)(LegendTitleWidth - LegendThickness - SmallMargin));

            switch (position)
            {
                case Position.Left:
                case Position.Right:
                    PlotArea = PlotArea.WithWidth(PlotArea.Width - Margin - LegendThickness - SmallMargin - maxWidth);
                    LegendTitleArea = new Rect(
                        PlotArea.Left + PlotArea.Width + Margin,
                        PlotArea.Top,
                        LegendTitleWidth,
                        LegendTitleHeight);
                    LegendArea = new Rect(
                        PlotArea.Left + PlotArea.Width + Margin,
                        PlotArea.Top + LegendTitleHeight + SmallMargin,
                        LegendThickness,
                        PlotArea.Height - LegendTitleHeight - SmallMargin);
                    break;

                case Position.Top:
                case Position.Bottom:
                    PlotArea = PlotArea.WithHeight(PlotArea.Height - Margin - LegendTitleHeight - SmallMargin - LegendThickness - SmallMargin - LegendTextHeight );
                    LegendTitleArea = new Rect(
                        PlotArea.Left + (PlotArea.Width - LegendTitleWidth) / 2,
                        PlotArea.Top + PlotArea.Height + Margin,
                        LegendTitleWidth,
                        LegendTitleHeight);
                    LegendArea = new Rect(
                        PlotArea.Left,
                        PlotArea.Top + PlotArea.Height + Margin + LegendTitleHeight + SmallMargin,
                        PlotArea.Width,
                        LegendThickness);
                    break;

                default:
                    break;
            }
        }
        #endregion

        #region Printers
        public Excel.Shape PrintText(double left, double top, object text, bool bold = false, StringFormater formater = null)
        {
            Excel.Shape box = Chart.Shapes.AddShape(
                                        Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle,
                                        (float)left, (float)top, 1f, 1f);

            box.Line.Visible = GetState(false);
            box.TextFrame2.MarginBottom = 0f;
            box.TextFrame2.MarginTop = 0f;
            box.TextFrame2.MarginLeft = 0f;
            box.TextFrame2.MarginRight = 0f;
            box.Fill.ForeColor.RGB = Color.White.ToRgb();
            box.Fill.Transparency = 1f;
            box.TextFrame.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            box.TextFrame.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            box.TextFrame.Characters().Font.Name = DefaultFontFamily;
            box.TextFrame.Characters().Font.Size = DefaultFontSize;
            box.TextFrame.Characters().Font.Bold = bold;
            box.TextFrame.Characters().Font.Color = Color.Black.ToRgb();
            string formattedText = formater == null ? (string)text : formater.Format(text);
            box.TextFrame.Characters().Text = formattedText;
            if (!String.IsNullOrEmpty(formattedText))
                box.TextFrame.AutoSize = true;

            box.Left = (float)left;
            box.Top = (float)top;
            return box;
        }
        #endregion

        #region Helpers
        public Microsoft.Office.Core.MsoTriState GetState(bool value)
        {
            if (value)
                return Microsoft.Office.Core.MsoTriState.msoTrue;
            else
                return Microsoft.Office.Core.MsoTriState.msoFalse;
        }
        #endregion
    }
}
