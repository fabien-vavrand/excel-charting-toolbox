﻿using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
using Toolbox.Charts;
using Toolbox.Charts.Treemap;
using Toolbox.Controls;

namespace Toolbox.ViewModel.Treemap
{
    public class TreemapViewModel : ViewModelBase
    {
        #region Properties
        public TreemapChart Treemap { get; set; }
		public ChartData Data { get; set; }
        public bool LockRefresh { get; set; }
        public bool IsDead { get; set; }
        #endregion

        #region View Properties
        private List<string> columns;
        public List<string> Columns
        {
            get { return columns; }
            set { Set("Columns", ref columns, value); }
        }

        private bool autoRefresh;
        public bool AutoRefresh
        {
            get { return autoRefresh; }
            set { Set("AutoRefresh", ref autoRefresh, value, broadcast: true); }
        }

        private bool showTitle;
        public bool ShowTitle
        {
            get { return showTitle; }
            set { Set("ShowTitle", ref showTitle, value, broadcast: true); }
        }

        public Wrapper<string> Title { get; set; }

        private TreemapAlgorithm algorithm;
        public TreemapAlgorithm Algorithm
        {
            get { return algorithm; }
            set { Set("Algorithm", ref algorithm, value, broadcast: true); }
        }

        private ObservableCollection<TreemapIndexViewModel> indexes;
        public ObservableCollection<TreemapIndexViewModel> Indexes
        {
            get { return indexes; }
            set { Set("Indexes", ref indexes, value); }
        }

        private TreemapIndexViewModel selectedIndex;
        public TreemapIndexViewModel SelectedIndex
        {
            get { return selectedIndex; }
            set { Set("SelectedIndex", ref selectedIndex, value); }
        }

        private string sizeColumn;
        public string SizeColumn
        {
            get { return sizeColumn; }
            set { Set("SizeColumn", ref sizeColumn, value, broadcast: true); }
        }

        private string colorColumn;
        public string ColorColumn
        {
            get { return colorColumn; }
            set { Set("ColorColumn", ref colorColumn, value, broadcast: true); }
        }

        private TreemapColorMethod colorMethod;
        public TreemapColorMethod ColorMethod
        {
            get { return colorMethod; }
            set { Set("ColorMethod", ref colorMethod, value, broadcast: true); }
        }

        private ViewModelBase colorViewModel;
        public ViewModelBase ColorViewModel
        {
            get { return colorViewModel; }
            set { Set("ColorViewModel", ref colorViewModel, value); }
        }

        private bool showLegend;
        public bool ShowLegend
        {
            get { return showLegend; }
            set { Set("ShowLegend", ref showLegend, value, broadcast: true); }
        }

        private Drawing.Position legendPosition;
        public Drawing.Position LegendPosition
        {
            get { return legendPosition; }
            set { Set("LegendPosition", ref legendPosition, value, broadcast: true); }
        }

        private FormatType legendFormatType;
        public FormatType LegendFormatType
        {
            get { return legendFormatType; }
            set { Set("LegendFormatType", ref legendFormatType, value, broadcast: true); }
        }

        private bool showLegendDecimalPlaces;
        public bool ShowLegendDecimalPlaces
        {
            get { return showLegendDecimalPlaces; }
            set { Set("ShowLegendDecimalPlaces", ref showLegendDecimalPlaces, value, broadcast: true); }
        }

        private int legendDecimalPlaces;
        public int LegendDecimalPlaces
        {
            get { return legendDecimalPlaces; }
            set { Set("LegendDecimalPlaces", ref legendDecimalPlaces, value, broadcast: true); }
        }

        private Gradient3ColorsViewModel gradient3ColorsViewModel;
        private Gradient2ColorsViewModel gradient2ColorsViewModel;
        private ColorPaletteViewModel colorPaletteViewModel;
        #endregion

        #region List of Values
        public IEnumerable<KeyValuePair<TreemapAlgorithm, string>> TreemapAlgorithms
        {
            get { return Utils.EnumKeyValues<TreemapAlgorithm>(); }
        }

        public IEnumerable<KeyValuePair<TreemapColorMethod, string>> TreemapColorMethods
        {
            get { return Utils.EnumKeyValues<TreemapColorMethod>(); }
        }

        public IEnumerable<KeyValuePair<Drawing.Position, string>> LegendPositions
        {
            get { return Utils.EnumKeyValues<Drawing.Position>(); }
        }

        public IEnumerable<KeyValuePair<FormatType, string>> LegendTextFormats
        {
            get { return Utils.EnumKeyValues<FormatType>(); }
        }

        public IEnumerable<int> DecimalPlaces
        {
            get { return new List<int> { 0, 1, 2, 3, 4 }; }
        }
        #endregion

        #region Commands
        public RelayCommand<object> RefreshCommand { get; set; }
        public RelayCommand<object> AddCommand { get; set; }
        public RelayCommand<object> DeleteCommand { get; set; }
        #endregion

        #region Ctor
        [PreferredConstructor]
        public TreemapViewModel()
        {
            Indexes = new ObservableCollection<TreemapIndexViewModel>();
        }

        public TreemapViewModel(TreemapChart treemap, ChartData data, TreemapAlgorithm algo) : this()
        {
            Treemap = treemap;
            Data = data;
            Columns = Data.ColumnNames;

            Algorithm = algo;
            InitParameters();
            InitColorViewModels();
            SetColorViewModel();

            Messenger.Default.Register<PropertyChangedMessageBase>
            (
                 this, true,
                 (m) =>
                 {
                     if (IsDead || !IsSentBySelf(m.Sender))
                         return;

                     if (m.PropertyName == "ColorMethod")
                         SetColorViewModel();

                     ShowLegendDecimalPlaces = LegendFormatType != FormatType.Text;

                     if (!LockRefresh)
                         DrawChart();
                 }
            );

            RefreshCommand = new RelayCommand<object>(_ => DrawChart(true));

            DeleteCommand = new RelayCommand<object>(
                _ =>
                {
                    LockRefresh = true;
                    Indexes.RemoveAt(Indexes.Count - 1);
                    Indexes.Last().AsChildIndex();

                    DeleteCommand.RaiseCanExecuteChanged();
                    AddCommand.RaiseCanExecuteChanged();

                    DrawChart();
                    LockRefresh = false;
                },
                _ => Indexes.Count > 1);

            AddCommand = new RelayCommand<object>(
                _ =>
                {
                    LockRefresh = true;
                    Indexes.Last().AsParentIndex();

                    string freeColumn = Columns.Where(c => !Indexes.Select(i => i.Column).Contains(c)).First();
                    Indexes.Add(new TreemapIndexViewModel(freeColumn));

                    DeleteCommand.RaiseCanExecuteChanged();
                    AddCommand.RaiseCanExecuteChanged();

                    DrawChart();
                    LockRefresh = false;
                },
                _ => Indexes.Count < Columns.Count);

            DrawChart();
        }

        private bool IsSentBySelf(object sender)
        {
            if (sender is TreemapViewModel && (TreemapViewModel)sender != this)
                return false;

            if (sender is TreemapIndexViewModel && Indexes.All(i => (TreemapIndexViewModel)sender != i))
                return false;

            if (sender is Gradient2ColorsViewModel && (Gradient2ColorsViewModel)sender != gradient2ColorsViewModel)
                return false;

            if (sender is Gradient3ColorsViewModel && (Gradient3ColorsViewModel)sender != gradient3ColorsViewModel)
                return false;

            if (sender is ColorPaletteViewModel && (ColorPaletteViewModel)sender != colorPaletteViewModel)
                return false;

            return true;
        }

        private void InitParameters()
        {
            ShowTitle = true;
            Title = new Wrapper<string>((o) => Tuple.Create(true, (o ?? String.Empty).ToString()));
            Title.Value = String.Empty;
            AutoRefresh = true;

            Indexes.Add(new TreemapIndexViewModel(Columns.First()));
            SelectedIndex = Indexes.Last();

            if (Columns.Count >= 3)
            {
                SizeColumn = Columns[Columns.Count - 2];
                ColorColumn = Columns.Last();
                ColorMethod = TreemapColorMethod.Gradient3Colors;
            }
            else
            {
                SizeColumn = Columns.Last();
                ColorColumn = Columns.First();
                ColorMethod = TreemapColorMethod.Palette;
            }

            ShowLegend = true;
            LegendPosition = Drawing.Position.Right;
            LegendFormatType = FormatType.Text;
            ShowLegendDecimalPlaces = false;
            LegendDecimalPlaces = 1;
        }
        #endregion

        #region Models
        private void InitColorViewModels()
        {
            gradient3ColorsViewModel = new Gradient3ColorsViewModel()
            {
                LowColor = Color.FromArgb(255, 255, 0, 0),
                MidColor = Color.FromArgb(255, 255, 255, 255),
                HighColor = Color.FromArgb(255, 0, 255, 0)
            };

            gradient2ColorsViewModel = new Gradient2ColorsViewModel()
            {
                LowColor = Color.FromArgb(255, 255, 255, 255),
                HighColor = Color.FromArgb(255, 0, 0, 255)
            };

            colorPaletteViewModel = new ColorPaletteViewModel();
        }

        private void SetColorViewModel()
        {
            switch (ColorMethod)
            {
                case TreemapColorMethod.Gradient3Colors:
                    ColorViewModel = gradient3ColorsViewModel.InitValues(Data.GetValues<double>(ColorColumn));
                    break;
                case TreemapColorMethod.Gradient2Colors:
                    ColorViewModel = gradient2ColorsViewModel.InitValues(Data.GetValues<double>(ColorColumn));
                    break;
                case TreemapColorMethod.Palette:
                    ColorViewModel = colorPaletteViewModel;
                    break;
                default:
                    break;
            }
        }

        private IColorSelector GetColorModel()
        {
            switch (ColorMethod)
            {
                case TreemapColorMethod.Gradient3Colors:
                    return gradient3ColorsViewModel.GetColorGradient();
                case TreemapColorMethod.Gradient2Colors:
                    return gradient2ColorsViewModel.GetColorGradient();
                case TreemapColorMethod.Palette:
                    return colorPaletteViewModel.GetColorPalette(Data.GetValues<string>(ColorColumn));
                default:
                    return null;
            }
        }
        #endregion

        #region Draw Chart
        public void DrawChart(bool refresh = false)
        {
            if (refresh)
            {
                Treemap.BuildAndDraw();
            }
            else
            {
                var indx = Indexes.Select(i => Data.GetValues<string>(i.Column)).ToList();
                var size = Data.GetValues<double>(SizeColumn).ToList();
                var color = Data.GetValues<object>(ColorColumn);

                TreemapParameters parameters = GetParameters();
                parameters.AutoRefresh = AutoRefresh;

                Treemap.DrawChart(indx, size, color, parameters);
            }

            if (!Treemap.IsActive)
            {
                IsDead = true;
                Messenger.Default.Send(new NotificationMessage<ChartBase>(Treemap, "Chart has been unactivated"), "ChartUnactivated");
                MessageBox.Show("An error has occured during chart rendering.", "Unexpected error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private TreemapParameters GetParameters()
        {
            TreemapParameters parameters = new TreemapParameters();
            parameters.ShowTitle = ShowTitle;
            parameters.Title = String.IsNullOrEmpty(Title.Value) ? SizeColumn : Title.Value;
            parameters.Algorithm = Algorithm;

            foreach (TreemapIndexViewModel index in Indexes)
                parameters.AddIndex(index.GetTreemapIndex());

            parameters.WithColor(GetColorModel());

            parameters.ShowLegend = ShowLegend;
            parameters.LegendTitle = ColorColumn;
            parameters.LegendPosition = LegendPosition;
            parameters.LegendBorder = parameters.Indexes.Last().GetLineOptions().With(o => o.Weight = o.Weight.Cap(1));
            parameters.LegendTextFormater.FormatType = LegendFormatType;
            parameters.LegendTextFormater.DecimalPlaces = LegendDecimalPlaces;
            return parameters;
        }
        #endregion
    }

    public enum TreemapColorMethod
    {
        [Description("3-Colors Gradient")]
        Gradient3Colors,
        [Description("2-Colors Gradient")]
        Gradient2Colors,
        [Description("Palette")]
        Palette
    }
}
