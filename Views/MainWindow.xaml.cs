using Microsoft.Win32;
using ProcedureScanner.Models;
using ProcedureScanner.Services;
using ScanProcedure.Models;
using ScanProcedure.Services;
using ScanProcedure.ViewModels;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;

namespace ScanProcedure
{
    /// <summary>
    /// Represents a file item in the navigation panel
    /// </summary>


    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainViewModel();
        }

    }
}