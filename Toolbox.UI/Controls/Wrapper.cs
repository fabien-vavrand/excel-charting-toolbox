using GalaSoft.MvvmLight;
using GalaSoft.MvvmLight.Command;
using GalaSoft.MvvmLight.Messaging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Toolbox.Controls
{
    public class Wrapper<T> : ViewModelBase
    {
        private T data;
        private bool cancelEdit;
        private Func<object, Tuple<bool,T>> converter;
        public RelayCommand<FrameworkElement> EnterCommand { get; set; }
        public RelayCommand<FrameworkElement> EscapeCommand { get; set; }

        public Wrapper(Func<object, Tuple<bool, T>> _converter)
        {
            converter = _converter;
            EnterCommand = new RelayCommand<FrameworkElement>((e) =>
            {
                e.Focus();
            });
            EscapeCommand = new RelayCommand<FrameworkElement>((e) => 
            {
                cancelEdit = true;
                e.Focus();
                cancelEdit = false;
            });
        }

        public object DirtyValue
        {
            get { return data; }
            set 
            {
                Tuple<bool, T> converted = converter(value);
                if (!cancelEdit && converted.Item1)
                    Set(ref data, converted.Item2, broadcast: true);
                else
                    RaisePropertyChanged("DirtyValue");
            }
        }

        public T Value
        {
            get { return data; }
            set
            {
                data = value;
                RaisePropertyChanged("DirtyValue");
            }
        }
    }
}
