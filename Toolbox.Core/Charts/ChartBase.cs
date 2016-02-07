using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Excel = Microsoft.Office.Interop.Excel;

namespace Toolbox.Charts
{
    public abstract class ChartBase
    {
        public Rect ChartArea { get; set; }
        public Excel.Chart Chart { get; set; }
        public List<Excel.Shape> Shapes { get; set; }
        public bool IsActive { get; set; }
    }
}
