using System.Windows;
using EmployeeManagerMVVM.ViewModels;

namespace EmployeeManagerMVVM
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }
    }
}