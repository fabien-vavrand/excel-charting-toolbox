using GalaSoft.MvvmLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Toolbox.Charts.Treemap;
using Toolbox.Drawing;
using Toolbox.Geometry;

namespace Toolbox.ViewModel.Treemap
{
    public class TreemapIndexViewModel : ViewModelBase
    {
        #region Properties
        private string column;
        public string Column
        {
            get { return column; }
            set { Set("Column", ref column, value, broadcast: true); }
        }

        private bool isParentIndex;
        public bool IsParentIndex
        {
            get { return isParentIndex; }
            set { Set("IsParentIndex", ref isParentIndex, value); }
        }

        private bool hasHeader;
        public bool HasHeader
        {
            get { return hasHeader; }
            set { Set("HasHeader", ref hasHeader, value, broadcast: true); }
        }

        private int margin;
        public int Margin
        {
            get { return margin; }
            set { Set("Margin", ref margin, value, broadcast: true); }
        }

        private Color fillColor;
        public Color FillColor
        {
            get { return fillColor; }
            set { Set("FillColor", ref fillColor, value, broadcast: true); }
        }

        private bool lineVisible;
        public bool LineVisible
        {
            get { return lineVisible; }
            set { Set("LineVisible", ref lineVisible, value, broadcast: true); }
        }

        private int lineWeight;
        public int LineWeight
        {
            get { return lineWeight; }
            set { Set("LineWeight", ref lineWeight, value, broadcast: true); }
        }

        private Color lineColor;
        public Color LineColor
        {
            get { return lineColor; }
            set { Set("LineColor", ref lineColor, value, broadcast: true); }
        }

        private int fontSize;
        public int FontSize
        {
            get { return fontSize; }
            set { Set("FontSize", ref fontSize, value, broadcast: true); }
        }

        private Color fontColor;
        public Color FontColor
        {
            get { return fontColor; }
            set { Set("FontColor", ref fontColor, value, broadcast: true); }
        }

        private bool fontBold;
        public bool FontBold
        {
            get { return fontBold; }
            set { Set("FontBold", ref fontBold, value, broadcast: true); }
        }

        private bool fontOutline;
        public bool FontOutline
        {
            get { return fontOutline; }
            set { Set("FontOutline", ref fontOutline, value, broadcast: true); }
        }

        private int fontOutlineWeight;
        public int FontOutlineWeight
        {
            get { return fontOutlineWeight; }
            set { Set("FontOutlineWeight", ref fontOutlineWeight, value, broadcast: true); }
        }

        private Color fontOutlineColor;
        public Color FontOutlineColor
        {
            get { return fontOutlineColor; }
            set { Set("FontOutlineColor", ref fontOutlineColor, value, broadcast: true); }
        }
        #endregion  

        #region List of Values
        public IEnumerable<int> Sizes
        {
            get
            {
                return new List<int>
                {
                    1, 2, 3, 4, 5, 6, 7, 8, 9
                };
            }
        }

        public IEnumerable<int> Margins
        {
            get
            {
                return new List<int>
                {
                    0, 1, 2, 3, 4, 5, 6, 7, 8, 9
                };
            }
        }

        public IEnumerable<int> FontSizes
        {
            get
            {
                return new List<int>
                {
                    8, 9, 10, 11, 12, 14, 16, 18, 
                    20, 22, 24, 26, 28, 36, 48, 72
                };
            }
        }
        #endregion

        #region Ctor
        public TreemapIndexViewModel(string _column)
        {
            Column = _column;

            HasHeader = false;
            Margin = 4;
            FillColor = Color.FromRgb(230, 230, 230);
            LineColor = Color.FromRgb(255, 255, 255);
            FontOutlineColor = Color.FromRgb(0, 0, 0);
            FontOutlineWeight = 1;

            AsChildIndex();
        }

        public TreemapIndexViewModel AsParentIndex()
        {
            IsParentIndex = true;
            LineWeight = 2;
            FontSize = 18;
            FontColor = Color.FromRgb(255, 255, 255);
            FontBold = true;
            FontOutline = true;
            return this;
        }

        public TreemapIndexViewModel AsChildIndex()
        {
            IsParentIndex = false;
            LineVisible = true;
            LineWeight = 1;
            FontSize = 12;
            FontColor = Color.FromRgb(0, 0, 0);
            FontBold = false;
            FontOutline = false;
            return this;
        }
        #endregion

        #region ToModel
        public TreemapIndex GetTreemapIndex()
        {
            TreemapIndex index = new TreemapIndex();
            index.HasHeader = HasHeader;
            index.Padding = new Margin(Margin);
            index.FillColor = System.Drawing.Color.FromArgb(FillColor.R, FillColor.G, FillColor.B);

            index.LineVisible = LineVisible;
            index.LineWeight = LineWeight;
            index.LineColor = System.Drawing.Color.FromArgb(LineColor.R, LineColor.G, LineColor.B);

            index.FontSize = FontSize;
            index.FontColor = System.Drawing.Color.FromArgb(FontColor.R, FontColor.G, FontColor.B);
            index.FontBold = FontBold;

            index.FontOutline = FontOutline;
            index.FontOutlineColor = System.Drawing.Color.FromArgb(FontOutlineColor.R, FontOutlineColor.G, FontOutlineColor.B);
            index.FontOutlineWeight = FontOutlineWeight;

            index.FontGlowRadius = 0;
            index.FontGlowColor = System.Drawing.Color.Transparent;
            return index;
        }
        #endregion
    }
}
