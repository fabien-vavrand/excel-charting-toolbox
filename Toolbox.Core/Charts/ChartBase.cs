using System.Collections.Generic;
using System.Linq;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;
using Toolbox.Drawing;
using System.Drawing;
using System;
using Toolbox.Geometry;
using Microsoft.Office.Core;

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
        protected void BuildArea()
        {
            PlotArea = new Rect(
                Margin,
                Margin,
                Math.Max(ChartArea.Width - 2 * Margin - Offset, 0),
                Math.Max(ChartArea.Height - 2 * Margin - Offset, 0));
        }

        protected void BuildTitle(ParametersBase parameters)
        {
            if (parameters.ShowTitle)
            {
                TitleArea = new Rect(
                    PlotArea.Left,
                    0,
                    PlotArea.Width,
                    30);
                PlotArea = PlotArea
                    .WithTop(PlotArea.Top + TitleArea.Height)
                    .WithHeight(Math.Max(PlotArea.Height - TitleArea.Height, 0));
            }
            else
            {
                TitleArea = new Rect(PlotArea.Left, 0, 0, 0);
            }
        }

        protected void BuildLegend(ParametersBase parameters)
        {
            LegendTitle = parameters.LegendTitle;
            IColorSelector color = parameters.Color;

            if (String.IsNullOrEmpty(LegendTitle))
                LegendTitle = "Color";
            if (LegendTitle.Length > 15)
                LegendTitle = LegendTitle.Substring(0, 13) + "...";

            float maxWidth = 0f;
            List<string> legendTexts = new List<string>();

            if (color is ColorGradient)
            {
                ColorGradient gradient = color as ColorGradient;
                legendTexts = gradient.Stops.Select(s => parameters.LegendTextFormater.Format(s.Value)).ToList();
            }
            else if (color is ColorPalette)
            {
                ColorPalette palette = color as ColorPalette;
                legendTexts = palette.Colors.Keys.Select(s => parameters.LegendTextFormater.Format(s)).ToList();
            }

            var sizes = legendTexts.Select(t => DefaultFont.RenderText(t));
            maxWidth = sizes.Max(s => s.Width);
            LegendTextHeight = sizes.Max(s => s.Height);

            //Legend title
            var titleSize = DefaultFontBold.RenderText(LegendTitle);
            LegendTitleWidth = titleSize.Width;
            LegendTitleHeight = titleSize.Height;

            if (parameters.LegendPosition == Position.Right || parameters.LegendPosition == Position.Left)
                maxWidth = Math.Max(maxWidth, (float)(LegendTitleWidth - LegendThickness - SmallMargin));

            switch (parameters.LegendPosition)
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
                        Math.Max(PlotArea.Height - LegendTitleHeight - SmallMargin, 0));
                    break;

                case Position.Top:
                case Position.Bottom:
                    PlotArea = PlotArea.WithHeight(PlotArea.Height - Margin - LegendTitleHeight - SmallMargin - LegendThickness); 
                    if (color is ColorGradient)
                        PlotArea = PlotArea.WithHeight(PlotArea.Height - SmallMargin - LegendTextHeight);

                    LegendTitleArea = new Rect(
                        Math.Max(PlotArea.Left + (PlotArea.Width - LegendTitleWidth) / 2, 0),
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

        public bool IsChartDegenerated()
        {
            return ChartArea.IsDegenerated() || PlotArea.IsDegenerated();
        }
        #endregion

        #region Drawers
        protected void GroupShapes()
        {
            if (Shapes.Count >= 2)
                Chart.Shapes.Range[Shapes.Select(s => s.Name).ToArray()].Group();
        }

        protected void DeleteShapes()
        {
            Shapes.ForEach(s => s.Delete());
            Shapes = new List<Excel.Shape>();
        }

        protected Excel.Shape AddShape(MsoAutoShapeType shapeType, Rect rect, Color color)
        {
            Excel.Shape shape = Chart.Shapes.AddShape(shapeType,
                                (float)rect.Left, (float)rect.Top,
                                (float)rect.Width, (float)rect.Height);

            shape.Fill.ForeColor.RGB = color.ToRgb();
            shape.Fill.Transparency = color.GetAlpha();
            Shapes.Add(shape);
            return shape;
        }

        protected Excel.Shape AddRectangle(Rect rect, Color color)
        {
            return AddShape(MsoAutoShapeType.msoShapeRectangle, rect, color);
        }

        protected void SetShapeLine(Excel.Shape shape, LineOptions options)
        {
            shape.Line.Visible = GetState(false);
            if (options.Visible)
            {
                shape.Line.Visible = GetState(options.Visible);
                shape.Line.Weight = (float)options.Weight;
                shape.Line.ForeColor.RGB = options.Color.ToRgb();
            }
        }

        protected Excel.Shape AddText(double left, double top, Font font, string text)
        {
            Excel.Shape box = AddRectangle(new Rect(left, top, 1, 1), Color.Transparent);

            box.Line.Visible = GetState(false);
            box.TextFrame2.MarginBottom = 0f;
            box.TextFrame2.MarginTop = 0f;
            box.TextFrame2.MarginLeft = 0f;
            box.TextFrame2.MarginRight = 0f;
            box.TextFrame.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            box.TextFrame.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            box.TextFrame.Characters().Font.Name = font.Name;
            box.TextFrame.Characters().Font.Size = font.Size;
            box.TextFrame.Characters().Font.Bold = font.Style == FontStyle.Bold;
            box.TextFrame.Characters().Font.Color = Color.Black.ToRgb();
            box.TextFrame.Characters().Text = text;

            if (!String.IsNullOrEmpty(text))
                box.TextFrame.AutoSize = true;

            box.Left = (float)left;
            box.Top = (float)top;
            return box;
        }
        #endregion

        #region Title Drawer
        protected void DrawTitle(ParametersBase parameters)
        {
            if (!parameters.ShowTitle)
                return;

            Excel.Shape shape = AddRectangle(TitleArea, Color.Transparent);
            shape.Line.Visible = GetState(false);
            shape.TextFrame.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            shape.TextFrame.Characters().Text = parameters.Title;
            shape.TextFrame.Characters().Font.Size = 20;
            shape.TextFrame.Characters().Font.Color = Color.Black.ToRgb();
        }
        #endregion

        #region Legend Drawer
        protected void DrawLegend(ParametersBase parameters)
        {
            if (!parameters.ShowLegend)
                return;

            AddText(LegendTitleArea.Left, LegendTitleArea.Top, DefaultFontBold, LegendTitle);

            if (parameters.Color is ColorGradient)
                DrawColorGradientLegend(parameters);

            else if (parameters.Color is ColorPalette)
                DrawColorPaletteLegend(parameters);
        }

        private void DrawColorGradientLegend(ParametersBase Parameters)
        {
            Excel.Shape shape = AddRectangle(LegendArea, Color.White);
            SetShapeLine(shape, Parameters.LegendBorder);

            ColorGradient gradient = Parameters.Color as ColorGradient;
            Excel.FillFormat fill = shape.Fill;

            float startPosition = 1;
            float midPosition = 0;
            float endPosition = 0;

            switch (Parameters.LegendPosition)
            {
                case Position.Left:
                case Position.Right:
                    fill.TwoColorGradient(MsoGradientStyle.msoGradientHorizontal, 1);
                    break;

                case Position.Top:
                case Position.Bottom:
                    fill.TwoColorGradient(MsoGradientStyle.msoGradientVertical, 1);
                    startPosition = 0;
                    endPosition = 1;
                    break;
            }

            fill.GradientStops[1].Position = startPosition;
            fill.GradientStops[1].Color.RGB = gradient.Stops.First().Color.ToRgb();
            fill.GradientStops[2].Position = endPosition;
            fill.GradientStops[2].Color.RGB = gradient.Stops.Last().Color.ToRgb();

            DrawGradientLegendText(Parameters, startPosition, gradient.Stops.First().Value);
            DrawGradientLegendText(Parameters, endPosition, gradient.Stops.Last().Value);

            if (gradient.Stops.Count == 3)
            {
                midPosition = (float)((gradient.Stops[1].Value - gradient.Stops.Last().Value) / (gradient.Stops.First().Value - gradient.Stops.Last().Value));
                if (Parameters.LegendPosition == Position.Bottom || Parameters.LegendPosition == Position.Top)
                    midPosition = 1 - midPosition;
                fill.GradientStops.Insert(gradient.Stops[1].Color.ToRgb(), midPosition, Index: 2);
                DrawGradientLegendText(Parameters, midPosition, gradient.Stops[1].Value);
            }
        }

        private void DrawColorPaletteLegend(ParametersBase Parameters)
        {
            ColorPalette palette = Parameters.Color as ColorPalette;
            double top = LegendArea.Top;
            double left = LegendArea.Left;
            double size = Math.Min(LegendArea.Width, LegendArea.Height);

            foreach (var color in palette.Colors)
            {
                string text = Parameters.LegendTextFormater.Format(color.Key);
                SizeF textSize = DefaultFont.RenderText(text);

                if (Parameters.LegendPosition == Position.Top || Parameters.LegendPosition == Position.Bottom)
                {
                    if (left + size + SmallMargin + textSize.Width > LegendArea.Left + LegendArea.Width)
                        break;
                }
                else if (Parameters.LegendPosition == Position.Left || Parameters.LegendPosition == Position.Right)
                {
                    if (top + size > LegendArea.Top + LegendArea.Height)
                        break;
                }

                Excel.Shape shape = AddRectangle(new Rect(left, top + textSize.Height / 2 - size / 2, size, size), color.Value);
                SetShapeLine(shape, Parameters.LegendBorder);
                AddText(left + size + SmallMargin, top, DefaultFont, text);

                switch (Parameters.LegendPosition)
                {
                    case Position.Left:
                    case Position.Right:
                        top += size + SmallMargin;
                        break;

                    case Position.Top:
                    case Position.Bottom:
                        left += size + SmallMargin + textSize.Width + SmallMargin;
                        break;
                }
            }
        }

        private void DrawGradientLegendText(ParametersBase Parameters, double position, double text)
        {
            Excel.Shape legend = null;
            switch (Parameters.LegendPosition)
            {
                case Position.Left:
                case Position.Right:
                    legend = AddText(LegendArea.Left + LegendArea.Width + SmallMargin, LegendArea.Top + position * LegendArea.Height,
                        DefaultFont, Parameters.LegendTextFormater.Format(text));
                    break;

                case Position.Top:
                case Position.Bottom:
                    legend = AddText(LegendArea.Left + position * LegendArea.Width, LegendArea.Top + LegendArea.Height + SmallMargin,
                        DefaultFont, Parameters.LegendTextFormater.Format(text));
                    break;
            }

            switch (Parameters.LegendPosition)
            {
                case Position.Left:
                case Position.Right:
                    if (position == 0)
                        legend.Top = (float)LegendArea.Top;
                    else if (position == 1)
                        legend.Top = (float)(LegendArea.Top + LegendArea.Height) - legend.Height;
                    else
                        legend.Top = (float)(LegendArea.Top + position * LegendArea.Height) - legend.Height / 2;
                    break;

                case Position.Top:
                case Position.Bottom:
                    if (position == 0)
                        legend.Left = (float)LegendArea.Left;
                    else if (position == 1)
                        legend.Left = (float)(LegendArea.Left + LegendArea.Width) - legend.Width;
                    else
                        legend.Left = (float)(LegendArea.Left + position * LegendArea.Width) - legend.Width / 2;
                    break;
            }
        }
        #endregion

        #region Helpers
        protected Microsoft.Office.Core.MsoTriState GetState(bool value)
        {
            if (value)
                return Microsoft.Office.Core.MsoTriState.msoTrue;
            else
                return Microsoft.Office.Core.MsoTriState.msoFalse;
        }
        #endregion
    }
}
