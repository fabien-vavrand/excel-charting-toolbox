using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Tools.Ribbon;
using Toolbox.Charts.Treemap;
using Excel = Microsoft.Office.Interop.Excel;
using Toolbox.Charts;
using Toolbox.View;
using System.Windows;
using MahApps.Metro.Controls;
using System.Windows.Forms.Integration;
using System.Windows.Forms;
using Microsoft.Office.Core;
using Toolbox.ViewModel;
using Toolbox.ViewModel.Treemap;
using GalaSoft.MvvmLight.Messaging;
using Toolbox.Drawing;

namespace Toolbox
{
    public partial class Ribbon
    {
        #region Load
        private void Ribbon_Load(object sender, RibbonUIEventArgs e)
        {

        }
        #endregion

        #region Charts Buttons
        private void buttonTreemap_Click(object sender, RibbonControlEventArgs e)
        {
            InitTreemap(TreemapAlgorithm.Squarify);
        }

        private void buttonCircularTreemap_Click(object sender, RibbonControlEventArgs e)
        {
            InitTreemap(TreemapAlgorithm.Circular);
        }
        #endregion
        
        #region Parameters
        private void buttonParameters_Click(object sender, RibbonControlEventArgs e)
        {
            Excel.Chart chart = Globals.ThisAddIn.Application.ActiveChart;
            if (chart != null && Globals.ThisAddIn.Charts.Select(c => c.Chart).Contains(chart))
            {
                ChartBase ch = Globals.ThisAddIn.Charts.Where(c => c.Chart == chart).First();
                TreemapChart treemap = (TreemapChart)ch;
            }
        }
        #endregion
        
        #region Init
        private ChartData InitDataWithMessage()
        {
            ChartData data = InitData();

            if (data == null)
                System.Windows.Forms.MessageBox.Show(
                    "Invalid data selected. You have to select a range containing values.",
                    "Excel Charting Toolbox", MessageBoxButtons.OK, MessageBoxIcon.Warning);

            return data;
        }

        private ChartData InitData()
        {
            dynamic selection = Globals.ThisAddIn.Application.Selection;
            bool isRange = selection is Excel.Range;
            if (!isRange)
                return null;

            Excel.Range range = selection;
            object[,] values = GetConcatenatedRangeValues(range);
            if (values == null)
                return null;

            return new ChartData(values);
        }

        private object[,] GetConcatenatedRangeValues(Excel.Range range)
        {
            Excel.Range lastCell = range.Worksheet.Cells.SpecialCells(Excel.XlCellType.xlCellTypeLastCell);
            object[,] values = GetRangeValues(range, lastCell);

            for (int i = 2; i <= range.Areas.Count; i++)
            {
                object[,] area = GetRangeValues(range.Areas[i], lastCell);

                if (values == null)
                    values = area;
                else if (area != null)
                    Utils.Concatenate(ref values, area);
            }

            return values;
        }

        private object[,] GetRangeValues(Excel.Range area, Excel.Range lastCell)
        {
            int row = Math.Min(area.Row + area.Rows.Count - 1, lastCell.Row) - area.Row + 1;
            int col = Math.Min(area.Column + area.Columns.Count - 1, lastCell.Column) - area.Column + 1;

            if (row <= 0 || col <= 0)
                return null;

            Excel.Range range = area.Worksheet.Range[area[1, 1], area[row, col]];
            return range.get_Value();
        }

        private Excel.Chart InitChart()
        {
            Excel.Range visible = Globals.ThisAddIn.Application.ActiveWindow.VisibleRange;
            double width = 500;
            double height = 400;
            double left = Math.Max(visible.Left + (visible.Width - 400) / 2 - width / 2, 0);
            double top = Math.Max(visible.Top + visible.Height / 2 - height / 2, 0);

            Excel.Range range = Globals.ThisAddIn.Application.Selection;
            Excel.ChartObjects cos = range.Worksheet.ChartObjects();
            Excel.ChartObject co = cos.Add(left, top, width, height);
            Excel.Chart chart = co.Chart;
            return chart;
        }

        private void InitTreemap(TreemapAlgorithm algorithm)
        {
            ChartData data = InitDataWithMessage();
            if (data == null)
                return;

            Excel.Chart chart = InitChart();
            TreemapChart treemap = new TreemapChart(chart);

            Globals.ThisAddIn.SetTaskPaneViewModel(new TreemapViewModel(treemap, data, algorithm));
            Globals.ThisAddIn.Charts.Add(treemap);
        }
        #endregion

        #region Sample Data
        private void buttonDataSet1_Click_1(object sender, RibbonControlEventArgs e)
        {
            GenerateTestData(25);
        }

        private void buttonDataSet2_Click(object sender, RibbonControlEventArgs e)
        {
            GenerateTestData(150);
        }

        private void buttonDataSet3_Click(object sender, RibbonControlEventArgs e)
        {
            GenerateTestData(1000);
        }

        private static void GenerateTestData(int n)
        {
            Excel.Worksheet sh = Globals.ThisAddIn.Application.ActiveSheet;
            var values = Enumerable.Range(1, n).ToList();
            Random rnd = new Random();
            var indexes = new List<List<string>>()
            {
                values.Select(i => "Value " + Math.Floor((double)(i-1)/20).ToString()).ToList(),
                values.Select(i => "Value " + i).ToList()
            };
            var size = values.Select(i => rnd.NextDouble()).ToList();
            var color = values.Select(i => rnd.NextDouble()).ToList();

            sh.Cells[1, 1].Value = "Dimension 1";
            sh.Cells[1, 2].Value = "Dimension 2";
            sh.Cells[1, 3].Value = "Measure 1";
            sh.Cells[1, 4].Value = "Measure 2";

            for (int i = 0; i < values.Count; i++)
            {
                sh.Cells[i + 2, 1].Value = indexes[0][i];
                sh.Cells[i + 2, 2].Value = indexes[1][i];
                sh.Cells[i + 2, 3].Value = size[i];
                sh.Cells[i + 2, 4].Value = color[i];
            }
        }
        #endregion
    }
}
