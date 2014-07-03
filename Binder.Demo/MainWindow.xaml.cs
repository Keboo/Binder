
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using GalaSoft.MvvmLight.Command;

namespace Binder.Demo
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        private readonly RelayCommand<Type> _selectTypeCommand;
        public MainWindow()
        {
            InitializeComponent();
            _selectTypeCommand = new RelayCommand<Type>(OnSelectType);
        }

        public IEnumerable<Type> Windows
        {
            get
            {
                var mainWindowType = typeof(MainWindow);
                var windowType = typeof(Window);
                return mainWindowType.Assembly.GetTypes().Where(x => windowType.IsAssignableFrom(x) && x != mainWindowType);
            }
        }

        public ICommand SelectTypeCommand
        {
            get { return _selectTypeCommand; }
        }

        private void OnSelectType(Type type)
        {
            Window window = (Window)Activator.CreateInstance(type);
            window.Show();
        }
    }
}
