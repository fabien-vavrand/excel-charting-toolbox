using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Excel = Microsoft.Office.Interop.Excel;
using System.Diagnostics;
using System.Drawing;
using Toolbox.Drawing;
using System.Windows;
using Toolbox.Geometry;

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
        public Microsoft.Office.Core.MsoAutoShapeType ShapeType { get; set; }

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

        public TreemapChart DrawChart(List<List<string>> indexes, List<double> size, List<object> colors, TreemapParameters parameters)
        {
            if (Chart == null)
                throw new Exception("Excel Chart should be initiliazed in order to Draw Chart");

            Indexes = indexes;
            Sizes = size;
            Colors = colors;
            Parameters = parameters;

            CompileInputs();
            SetTreemapData();
            AutoBuildAndDraw();

            return this;
        }
        #endregion

        #region CompileInputs
        private void CompileInputs()
        {
            if (Parameters == null || Indexes == null || Sizes == null || Colors == null)
                throw new ArgumentNullException("Undefined indexes parameters (some indexes do not have parameters)");
            
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

        private void SetTreemapData()
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
        public TreemapChart Build(Excel.Range range)
        {
            return Build(range.Left, range.Top, range.Width, range.Height);
        }

        public TreemapChart Build(double left, double top, double width, double height)
        {
            ChartArea = new Rect(left, top, width, height);

            BuildArea();
            BuildTitle(Parameters);
            BuildLegend(Parameters);

            if (Parameters.Algorithm == TreemapAlgorithm.Circular)
            {
                double edge = Math.Min(PlotArea.Width, PlotArea.Height);
                double excessX = (PlotArea.Width - edge) / 2;
                double excessY = (PlotArea.Height - edge) / 2;
                PlotArea = new Rect(PlotArea.X + excessX, PlotArea.Y + excessY, edge, edge);
            }

            if (IsChartDegenerated())
                return this;

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

                if (index.HasHeader && Parameters.Algorithm != TreemapAlgorithm.Circular)
                {
                    FontStyle style = index.FontBold ? FontStyle.Bold : FontStyle.Regular;
                    Font font = new Font(DefaultFontFamily, (float)index.FontSize, style, GraphicsUnit.Pixel);
                    float height = font.RenderText(item.Indexes.Last()).Height;
                    item.SetMargin(new Margin(index.Padding.Left, height, index.Padding.Right, index.Padding.Bottom));
                }

                if (i == Indexes.Count - 1)
                    item.FillColor = Parameters.Color.GetColor(item.Color);
                else
                    item.FillColor = index.FillColor;
            }
        }

        #endregion

        #region Drawer
        public TreemapChart Draw(Excel.Worksheet sheet)
        {
            Excel.ChartObjects cos = sheet.ChartObjects();
            Excel.ChartObject co = cos.Add(ChartArea.Left, ChartArea.Top, ChartArea.Width, ChartArea.Height);
            Chart = co.Chart;
            Draw();
            return this;
        }

        public void BuildAndDraw()
        {
            if (!IsActive)
                return;
            
            try
            {
                Build(Chart.ChartArea.Left, Chart.ChartArea.Top, Chart.ChartArea.Width, Chart.ChartArea.Height);
                Draw();

                if (!resizeListened)
                {
                    Chart.Resize += AutoBuildAndDraw;
                    resizeListened = true;
                }
            }
            catch (Exception e)
            {
                IsActive = false;
            }
        }

        private void AutoBuildAndDraw()
        {
            if (!Parameters.AutoRefresh)
                return;

            BuildAndDraw();
        }

        private void Draw()
        {
            DeleteShapes();

            if (IsChartDegenerated())
                return;

            SetShapeType();
            Draw(Parent);
            DrawTitle(Parameters);
            DrawLegend(Parameters);
        }

        private void SetShapeType()
        {
            ShapeType = Microsoft.Office.Core.MsoAutoShapeType.msoShapeRectangle;
            if (Parameters.Algorithm == TreemapAlgorithm.Circular)
                ShapeType = Microsoft.Office.Core.MsoAutoShapeType.msoShapeOval;
        }

        private void Draw(TreemapItem item)
        {
            TreemapIndex index = item.IndexParameters;

            //Add background shapes
            if (!item.IsParent() && (index.HasHeader || item.IsChild() || Parameters.Algorithm == TreemapAlgorithm.Circular))
            {
                Excel.Shape shape = AddShape(ShapeType, item.Rectangle, item.FillColor);
                SetShapeLine(shape, index.GetLineOptions());
                SetShapeText(shape, item);
            }

            //Add Children Items
            foreach (var child in item.Items)
                Draw(child);

            //Add front shapes
            if (!item.IsParent() && !item.IsChild())
            {
                Excel.Shape frontShape = AddShape(ShapeType, item.InnerRectangle, Color.Transparent);

                if (Parameters.Algorithm == TreemapAlgorithm.Circular)
                {
                    frontShape.Line.Visible = GetState(false);
                    SetShapeText(frontShape, item);
                }  
                else if (!index.HasHeader)
                {
                    SetShapeLine(frontShape, index.GetLineOptions());
                    SetShapeText(frontShape, item);
                }
                else //Case Header : inner border & no text
                    SetShapeLine(frontShape, index.GetLineOptions().With(o => o.Weight = 1));
            }
        }

        private void SetShapeText(Excel.Shape shape, TreemapItem item)
        {
            TreemapIndex index = item.IndexParameters;

            shape.TextFrame2.WordWrap = Microsoft.Office.Core.MsoTriState.msoTrue;
            shape.TextFrame2.MarginBottom = 0.01f;
            shape.TextFrame2.MarginTop = 0.01f;
            shape.TextFrame2.MarginLeft = 0.01f;
            shape.TextFrame2.MarginRight = 0.01f;

            if (index.HasHeader && Parameters.Algorithm != TreemapAlgorithm.Circular)
                shape.TextFrame.VerticalAlignment = Excel.XlVAlign.xlVAlignTop;
            else
                shape.TextFrame.VerticalAlignment = Excel.XlVAlign.xlVAlignCenter;

            shape.TextFrame.HorizontalAlignment = Excel.XlHAlign.xlHAlignCenter;

            shape.TextFrame.Characters().Font.Bold = index.FontBold;
            shape.TextFrame.Characters().Font.Size = index.FontSize;
            shape.TextFrame.Characters().Font.Color = index.FontColor.ToRgb();

            if (index.FontOutline)
            {
                shape.TextFrame2.TextRange.Font.Line.Visible = GetState(index.FontOutline);
                shape.TextFrame2.TextRange.Font.Line.ForeColor.RGB = index.FontOutlineColor.ToRgb();
                shape.TextFrame2.TextRange.Font.Line.Weight = (float)index.FontOutlineWeight;
            }

            if (item.Items.Count == 0)
            {
                if (item.FillColor.GetBrightness() < 0.7)
                    shape.TextFrame.Characters().Font.Color = Color.White.ToRgb();
                else
                    shape.TextFrame.Characters().Font.Color = Color.Black.ToRgb();
            }

            string text = item.Indexes.Last();
            float size = (float)index.FontSize;
            SizeF textSize = new Font(DefaultFontFamily, size).RenderText(text);
            int lines = (int)Math.Floor(textSize.Width / item.Rectangle.Width) + 1;

            while (size > 1 && lines * textSize.Height > item.Rectangle.Height)
            {
                size--;
                textSize = new Font(DefaultFontFamily, size).RenderText(text);
                lines = (int)Math.Floor(textSize.Width / item.Rectangle.Width) + 1;
            }

            if (size > 3)
            {
                shape.TextFrame.Characters().Text = text;
                shape.TextFrame.Characters().Font.Size = size;
            }
        }
        #endregion
    }

    public class TreemapChart<D> : TreemapChart
    {
        public TreemapChart(List<D> data, 
            Func<D, List<string>> indexes, Func<D, double> size, Func<D, object> color,
            TreemapParameters parameters)
        {
            int n = indexes(data.First()).Count();
            Indexes = Enumerable.Range(0, n).Select(i => data.Select(indexes).ToList().GetRange(i, 1).First()).ToList();
            Sizes = data.Select(size).ToList();
            Colors = data.Select(color).ToList();
            Parameters = parameters;
        }

        new public TreemapChart Build(double left, double top, double width, double height)
        {
            return base.Build(left, top, width, height);
        }
    }
}
