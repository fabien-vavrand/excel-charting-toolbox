using GalaSoft.MvvmLight.Messaging;
using MahApps.Metro.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Toolbox.ViewModel;
using Toolbox.ViewModel.Treemap;

namespace Toolbox.View
{
    /// <summary>
    /// Interaction logic for TreemapView.xaml
    /// </summary>
    public partial class TreemapView : UserControl
    {
        public TreemapView(TreemapViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
