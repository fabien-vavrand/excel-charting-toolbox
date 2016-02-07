using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Ioc;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using Toolbox.Charts;
using Toolbox.Charts.Treemap;
using Toolbox.ViewModel;
using Toolbox.ViewModel.Treemap;

namespace Toolbox.DesignData.Treemap
{
    public class TreemapViewModelDesign : TreemapViewModel
    {
        public TreemapViewModelDesign()
        {
            TreemapIndexViewModel index1 = new TreemapIndexViewModel("Column 1");
            Indexes.Add(index1.AsParentIndex());
            TreemapIndexViewModel index2 = new TreemapIndexViewModel("Column 2");
            Indexes.Add(index2.AsChildIndex());

            SelectedIndex = index1;

            ColorViewModel = new Gradient3ColorsViewModel()
            {
                LowColor = Color.FromArgb(255, 255, 0, 0),
                MidColor = Color.FromArgb(255, 255, 255, 255),
                HighColor = Color.FromArgb(255, 0, 255, 0)
            }
            .InitValues(new List<double> { 0, 1, 2, 3, 4 });
        }
    }
}
