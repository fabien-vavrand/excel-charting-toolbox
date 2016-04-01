using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Drawing;
using Toolbox.Drawing;
using System.Windows;

namespace Toolbox.Charts.Treemap
{
    public class TreemapChart : ChartBase
    {
        #region Properties
        public TreemapParameters Parameters { get; set; }
        public List<List<string>> Indexes { get; set; }
        public List<double> Sizes { get; set; }
        public List<object> Colors { get; set; }
        public List<TreemapData> Data { get; set; }

        public TreemapItem Parent { get; set; }
        


        private bool resizeListened = false;
        #endregion

        #region Ctor
        public TreemapChart()
        {
            IsActive = true;
            Shapes = new List<Excel.Shape>();
        }

        public TreemapChart(Excel.Chart chart) : this()
        {
            Chart = chart;
        }

        public TreemapChart(TreemapParameters parameters)
            : this()
        {
            Parameters = parameters;
        }

        public TreemapChart(List<List<string>> indexes, List<double> size, List<object> colors, TreemapParameters parameters)
            : this()
        {
            Update(indexes, size, colors, parameters);
        }

        public void Update(List<List<string>> indexes, List<double> size, List<object> colors, TreemapParameters parameters)
        {
            Indexes = indexes;
            Sizes = size;
            Colors = colors;
            Parameters = parameters;

            CompileInputs();
            CompileTreemapData();

            BuildAndPrint();
        }
        #endregion

        #region CompileInputs
        private void CompileInputs()
        {
            if (Parameters == null || Indexes == null || Sizes == null || Colors == null)
                throw new ArgumentException("Undefined indexes parameters (some indexes do not have parameters)");
            
            if (Parameters.Indexes.Count < Indexes.Count)
                throw new ArgumentException("Undefined indexes parameters (some indexes do not have parameters)");

            if (Parameters.Indexes.Count > Indexes.Count)
                throw new ArgumentException("Undefined indexes (indexes parameters do not match indexes)");

            if (Parameters.Color == null)
                throw new ArgumentException("Undefined color parameter");

            int n = Sizes.Count;

            foreach (var index in Indexes)
                if (index.Count != n)
                    throw new ArgumentException("All input data should have the same size");

            if (n != Colors.Count)
                throw new ArgumentException("All input data should have the same size");
        }

        private void CompileTreemapData()
        {
            Data = new List<TreemapData>();

            for (int i = 0; i < Sizes.Count; i++)
            {
                TreemapData data = new TreemapData();
                foreach (var index in Indexes)
                    data.Indexes.Add(index[i]);

                data.Size = Math.Max(Sizes[i], 0);
                data.Color = Colors[i];
                Data.Add(data);
            }

            Data = Data.OrderByDescending(d => d.Size).ToList();
        }
        #endregion

        #region Builder
        public TreemapChart Build(double left, double top, double width, double height)
        {
            ChartArea = new Rect(left, top, width, height);

            BuildArea();

            if (Parameters.ShowTitle)
                BuildTitle();

            if (Parameters.ShowLegend)
                BuildLegend(Parameters.LegendTitle, Parameters.Color, Parameters.LegendPosition, Parameters.LegendTextFormater);

            if (Parameters.Algorithm == TreemapAlgorithm.Circular)
            {
                double edge = Math.Min(PlotArea.Width, PlotArea.Height);
                double excessX = (PlotArea.Width - edge) / 2;
                double excessY = (PlotArea.Height - edge) / 2;
                PlotArea = new Rect(PlotArea.X + excessX, PlotArea.Y + excessY, edge, edge);
            }

            Parent = new TreemapItem(PlotArea.Left, PlotArea.Top, PlotArea.Width, PlotArea.Height);
            Parent.Size = Sizes.Sum();
            Parent.IndexParameters = new TreemapIndex()
            {
                LineVisible = false,
                LineWeight = 0
            };

            IndexesComparer comparer = new IndexesComparer();
            List<TreemapItem> items = new List<TreemapItem>() { Parent };

            for (int i = 0; i < Indexes.Count; i++)
            {
                List<TreemapData> data = GetDepthData(i);

                foreach (TreemapItem item in items)
                {
                    List<TreemapData> itemData = data.Where(d => comparer.Equals(d.Indexes.Take(i).ToList(), item.Indexes)).ToList();
                    item.ApplyAlgorithm(itemData, Parameters.Algorithm);
                }

                items = items.SelectMany(item => item.Items).ToList();

                SetTreemapItemsParameters(i, items);
            }

            return this;
        }

        private List<TreemapData> GetDepthData(int i)
        {
            bool childLevel = i == Indexes.Count - 1;
            IEnumerable<TreemapData> data;

            if (!childLevel)
                data = Data.GroupBy(
                            d => d.Indexes.Take(i + 1).ToList(),
                            d => d.Size,
                            (k, g) => new TreemapData(k, g.Sum(), 0),
                            new IndexesComparer())
                        .OrderByDescending(d => d.Size);
            else
                data = Data;

            return data.Where(d => d.Size > 0).ToList();
        }

        private void SetTreemapItemsParameters(int i, List<TreemapItem> items)
        {
            TreemapIndex index = Parameters.Indexes[i];
            foreach (TreemapItem item in items)
            {
                item.IndexParameters = index;
                item.SetMargin(index.Padding);

                if (i == Indexes.Count - 1)
                    item.FillColor = Parameters.Color.GetColor(item.Color);
            }
        }

        #endregion
        
        #region Print
        public TreemapChart Print(Excel.Worksheet sheet)
        {
            Excel.ChartObjects cos = sheet.ChartObjects();
            Excel.ChartObject co = cos.Add(ChartArea.Left, ChartArea.Top, ChartArea.Width, ChartArea.Height);
            Chart = co.Chart;

            Shapes = Print(Parent);
            Shapes.AddRange(PrintTitle());
            Shapes.AddRange(PrintLegend());

            if (Shapes.Count >= 2)
                Chart.Shapes.Range[Shapes.Select(s => s.Name).ToArray()].Group();

            return this;
        }

        public void BuildAndPrint()
        {
            if (!IsActive)
                return;

            try
            {
                Shapes.ForEach(s => s.Delete());

                Build(Chart.ChartArea.Left, Chart.ChartArea.Top, Chart.ChartArea.Width, Chart.ChartArea.Height);

                Shapes = Print(Parent);
                Shapes.AddRange(PrintTitle());
                Shapes.AddRange(PrintLegend());

                if (Shapes.Count >= 2)
                    Chart.Shapes.Range[Shapes.Select(s => s.Name).ToArray()].Group();

                if (!resizeListened)
                {
                    Chart.Resize += Chart_Resize;
                    resizeListened = true;
                }
            }
            catch (Exception e)
            {
                IsActive = false;
            }

            return;
        }

        private void Chart_Resize()
        {
            BuildAndPrint();
        }

        private List<Excel.Shape> Print(TreemapItem tmItem)
        {
            List<Excel.Shape> shapes = new List<Excel.Shape>();

            foreach (var item in tmItem.Items)
                shapes.AddRange(Print(item));

            Microsoft.Office.Core.MsoAutoShapeType shapeType = Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle;
            if (Parameters.Algorithm == TreemapAlgorithm.Circular)
                shapeType = Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval;

            Excel.Shape shape = Chart.Shapes.AddShape(shapeType,
                    (float)tmItem.Rectangle.Left, (float)tmItem.Rectangle.Top,
                    (float)tmItem.Rectangle.Width, (float)tmItem.Rectangle.Height);

            if (tmItem.FillColor != null)
            {
                shape.Fill.ForeColor.RGB = tmItem.FillColor.ToRgb();
                shape.Fill.Transparency = tmItem.FillColor.GetAlpha();
            }
            else
            {
                shape.Fill.ForeColor.RGB = tmItem.IndexParameters.FillColor.ToRgb();
                shape.Fill.Transparency = tmItem.IndexParameters.FillColor.GetAlpha();
            }

            shape.Line.Visible = GetState(false);
            if (tmItem.IndexParameters.LineVisible)
            {
                shape.Line.Visible = GetState(tmItem.IndexParameters.LineVisible);
                shape.Line.Weight = (float)tmItem.IndexParameters.LineWeight;
                shape.Line.ForeColor.RGB = tmItem.IndexParameters.LineColor.ToRgb();
            }

            shape.TextFrame.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            shape.TextFrame.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;
            shape.TextFrame.Characters().Font.Bold = tmItem.IndexParameters.FontBold;
            shape.TextFrame.Characters().Font.Size = tmItem.IndexParameters.FontSize;
            shape.TextFrame.Characters().Font.Color = tmItem.IndexParameters.FontColor.ToRgb();
            shape.TextFrame2.WordWrap = Microsoft.Office.Core.MsoTriState.msoTrue;
            shape.TextFrame2.MarginBottom = 0.01f;
            shape.TextFrame2.MarginTop = 0.01f;
            shape.TextFrame2.MarginLeft = 0.01f;
            shape.TextFrame2.MarginRight = 0.01f;
            if (tmItem.IndexParameters.FontOutline)
            {
                shape.TextFrame2.TextRange.Font.Line.Visible = GetState(tmItem.IndexParameters.FontOutline);
                shape.TextFrame2.TextRange.Font.Line.ForeColor.RGB = tmItem.IndexParameters.FontOutlineColor.ToRgb();
                shape.TextFrame2.TextRange.Font.Line.Weight = (float)tmItem.IndexParameters.FontOutlineWeight;
                //shape.TextFrame2.TextRange.Font.Glow.Radius = (float)tmItem.IndexParameters.FontGlowRadius;
                //shape.TextFrame2.TextRange.Font.Glow.Color.RGB = tmItem.IndexParameters.FontGlowColor.ToRgb();
                //shape.TextFrame2.TextRange.Font.Glow.Transparency = tmItem.IndexParameters.FontGlowColor.GetAlpha();
            }

            if (tmItem.Items.Count == 0)
            {
                if (tmItem.FillColor.GetBrightness() < 0.5)
                    shape.TextFrame.Characters().Font.Color = Color.White.ToRgb();
                else
                    shape.TextFrame.Characters().Font.Color = Color.Black.ToRgb();
            }

            if (tmItem.Indexes.Count > 0)
            {
                string text = tmItem.Indexes.Last();
                float size = (float)tmItem.IndexParameters.FontSize;
                SizeF textSize = new Font(DefaultFontFamily, size).RenderText(text);
                int lines = (int)Math.Floor(textSize.Width / tmItem.Rectangle.Width) + 1;

                while (size > 1 && lines * textSize.Height > tmItem.Rectangle.Height)
                {
                    size--;
                    textSize = new Font(DefaultFontFamily, size).RenderText(text);
                    lines = (int)Math.Floor(textSize.Width / tmItem.Rectangle.Width) + 1;
                }

                if (size > 3)
                {
                    shape.TextFrame.Characters().Text = text;
                    shape.TextFrame.Characters().Font.Size = size;
                }
            }

            shapes.Add(shape);
            return shapes;
        }

        private List<Excel.Shape> PrintTitle()
        {
            List<Excel.Shape> shapes = new List<Excel.Shape>();

            if (!Parameters.ShowTitle)
                return shapes;

            Excel.Shape shape = Chart.Shapes.AddShape(
                    Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle,
                    (float)Parent.Rectangle.Left, 0,
                    (float)Parent.Rectangle.Width, 30f);
            shapes.Add(shape);

            shape.Line.Visible = GetState(false);
            shape.Fill.ForeColor.RGB = Color.White.ToRgb();
            shape.TextFrame.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;
            shape.TextFrame.Characters().Text = Parameters.Title;
            shape.TextFrame.Characters().Font.Size = 20;
            shape.TextFrame.Characters().Font.Color = Color.Black.ToRgb();

            return shapes;
        }

        private List<Excel.Shape> PrintLegend()
        {
            List<Excel.Shape> shapes = new List<Excel.Shape>();

            if (!Parameters.ShowLegend)
                return shapes;

            shapes.Add(PrintText(LegendTitleArea.Left, LegendTitleArea.Top, LegendTitle, bold: true));

            if (Parameters.Color is ColorGradient)
                PrintColorGradientLegend(shapes);

            else if (Parameters.Color is ColorPalette)
                PrintColorPaletteLegend(shapes);

            return shapes;
        }

        private void PrintColorGradientLegend(List<Excel.Shape> shapes)
        {
            Excel.Shape shape = Chart.Shapes.AddShape(
                                Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle,
                                (float)LegendArea.Left, (float)LegendArea.Top,
                                (float)LegendArea.Width, (float)LegendArea.Height);
            shapes.Add(shape);
            PrintLegendShapeBorder(shape);

            ColorGradient gradient = Parameters.Color as ColorGradient;
            Excel.FillFormat fill = shape.Fill;

            float startPosition = 1;
            float midPosition = 0;
            float endPosition = 0;

            switch (Parameters.LegendPosition)
            {
                case Position.Left:
                case Position.Right:
                    fill.TwoColorGradient(Microsoft.Office.Core.MsoGradientStyle.msoGradientHorizontal, 1);
                    break;

                case Position.Top:
                case Position.Bottom:
                    fill.TwoColorGradient(Microsoft.Office.Core.MsoGradientStyle.msoGradientVertical, 1);
                    startPosition = 0;
                    endPosition = 1;
                    break;
            }

            fill.GradientStops[1].Position = startPosition;
            fill.GradientStops[1].Color.RGB = gradient.Stops.First().Color.ToRgb();
            fill.GradientStops[2].Position = endPosition;
            fill.GradientStops[2].Color.RGB = gradient.Stops.Last().Color.ToRgb();

            shapes.Add(PrintGradientLegendText(Parameters.LegendPosition, startPosition, gradient.Stops.First().Value));
            shapes.Add(PrintGradientLegendText(Parameters.LegendPosition, endPosition, gradient.Stops.Last().Value));

            if (gradient.Stops.Count == 3)
            {
                midPosition = (float)((gradient.Stops[1].Value - gradient.Stops.Last().Value) / (gradient.Stops.First().Value - gradient.Stops.Last().Value));
                if (Parameters.LegendPosition == Position.Bottom || Parameters.LegendPosition == Position.Top)
                    midPosition = 1 - midPosition;
                fill.GradientStops.Insert(gradient.Stops[1].Color.ToRgb(), midPosition, Index: 2);
                shapes.Add(PrintGradientLegendText(Parameters.LegendPosition, midPosition, gradient.Stops[1].Value));
            }
        }

        private void PrintColorPaletteLegend(List<Excel.Shape> shapes)
        {
            ColorPalette palette = Parameters.Color as ColorPalette;
            double top = LegendArea.Top;
            double left = LegendArea.Left;
            double size = Math.Min(LegendArea.Width, LegendArea.Height);

            foreach (var color in palette.Colors)
            {
                if (Parameters.LegendPosition == Position.Top || Parameters.LegendPosition == Position.Bottom)
                {
                    if (left + size > LegendArea.Left + LegendArea.Width)
                        break;
                }
                else if(Parameters.LegendPosition == Position.Left || Parameters.LegendPosition == Position.Right)
                {
                    if (top + size > LegendArea.Top + LegendArea.Height)
                        break;
                }

                Excel.Shape text = PrintText(left + size + SmallMargin, top, color.Key, formater: Parameters.LegendTextFormater);
                shapes.Add(text);

                Excel.Shape shape = Chart.Shapes.AddShape(
                            Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle,
                            (float)left, (float)(top + text.Height / 2 - size / 2),
                            (float)size, (float)size);
                shapes.Add(shape);
                PrintLegendShapeBorder(shape);
                shape.Fill.ForeColor.RGB = color.Value.ToRgb();

                switch (Parameters.LegendPosition)
                {
                    case Position.Left:
                    case Position.Right:
                        top += size + SmallMargin;
                        break;

                    case Position.Top:
                    case Position.Bottom:
                        left += size + SmallMargin + text.Width + SmallMargin;
                        break;
                }
            }
        }

        public void PrintLegendShapeBorder(Excel.Shape shape)
        {
            shape.Line.Visible = GetState(Parameters.Indexes.Last().LineVisible);
            shape.Line.Weight = (float)Math.Min(Parameters.Indexes.Last().LineWeight, 1);
            shape.Line.ForeColor.RGB = Parameters.Indexes.Last().LineColor.ToRgb();
        }

        private Excel.Shape PrintGradientLegendText(Position legendPosition, double position, double text)
        {
            Excel.Shape legend = null;
            switch (legendPosition)
            {
                case Position.Left:
                case Position.Right:
                    legend = PrintText(LegendArea.Left + LegendArea.Width + SmallMargin, LegendArea.Top + position * LegendArea.Height, 
                        text, 
                        formater: Parameters.LegendTextFormater);
                    break;

                case Position.Top:
                case Position.Bottom:
                    legend = PrintText(LegendArea.Left + position * LegendArea.Width, LegendArea.Top + LegendArea.Height + SmallMargin, 
                        text, 
                        formater: Parameters.LegendTextFormater);
                    break;
            }

            switch (legendPosition)
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

            return legend;
        }
        #endregion
    }

    public class TreemapChart<D> : TreemapChart
    {
        public TreemapChart(List<D> data, 
            Func<D, List<string>> indexes, Func<D, double> size, Func<D, object> color,
            TreemapParameters parameters) : base(parameters)
        {
            int n = indexes(data.First()).Count();
            Indexes = Enumerable.Range(0, n).Select(i => data.Select(indexes).ToList().GetRange(i, 1).First()).ToList();
            Sizes = data.Select(size).ToList();
            Colors = data.Select(color).ToList();
        }

        new public TreemapChart Build(double left, double top, double width, double height)
        {
            return base.Build(left, top, width, height);
        }
    }
}
