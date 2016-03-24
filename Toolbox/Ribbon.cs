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

        #region Init Charts
        private void buttonTreemap_Click(object sender, RibbonControlEventArgs e)
        {
            ChartData data = InitData();
            if (data == null)
                return;

            Excel.Chart chart = InitChart();
            InitTreemap(data, chart, TreemapAlgorithm.Squarify);
        }

        private void buttonCircularTreemap_Click(object sender, RibbonControlEventArgs e)
        {
            ChartData data = InitData();
            if (data == null)
                return;

            Excel.Chart chart = InitChart();
            InitTreemap(data, chart, TreemapAlgorithm.Circular);
        }

        private static void InitTreemap(ChartData data, Excel.Chart chart, TreemapAlgorithm algorithm)
        {
            TreemapChart treemap = new TreemapChart(chart);
            Globals.ThisAddIn.SetTaskPaneViewModel(new TreemapViewModel(treemap, data, algorithm));
            Globals.ThisAddIn.Charts.Add(treemap);
        }

        private Excel.Chart InitChart()
        {
            Excel.Range visible = Globals.ThisAddIn.Application.ActiveWindow.VisibleRange;
            double width = 600;
            double height = 360;
            double left = visible.Left + (visible.Width - 400) / 2 - width / 2;
            double top = visible.Top + visible.Height / 2 - height / 2;

            Excel.Range range = Globals.ThisAddIn.Application.Selection;
            Excel.ChartObjects cos = range.Worksheet.ChartObjects();
            Excel.ChartObject co = cos.Add(left, top, width, height);
            Excel.Chart chart = co.Chart;
            return chart;
        }

        private ChartData InitData()
        {
            Excel.Range range = Globals.ThisAddIn.Application.Selection;
            if (range.Count <= 1 || range.Count > 10000)
                return null;

            ChartData data = new ChartData(range.Value2);
            return data;
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

            sh.Cells[1, 1].Value = "Column 1";
            sh.Cells[1, 2].Value = "Column 2";
            sh.Cells[1, 3].Value = "Column 3";
            sh.Cells[1, 4].Value = "Column 4";

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
