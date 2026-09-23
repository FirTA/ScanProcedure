using ProcedureScanner.Models;
using ScanProcedure.Models;
using ScanProcedure.ViewModels;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

namespace ScanProcedure.Views
{
    /// <summary>
    /// Interaction logic for ScanProcedureView.xaml
    /// </summary>
    public partial class ScanProcedureView : UserControl
    {
        private ObservableCollection<FileItem> _fileItems = new ObservableCollection<FileItem>();

        public ScanProcedureView()
        {
            InitializeComponent();
            var items = Enum.GetNames<DatabaseType>().Select(name => new Item { Name = name }).ToList();
            DbPicker.ItemsSource = items;
            DbPicker.SelectedItem = items[0];

            // Bind the list box to the collection
            FileListBox.ItemsSource = _fileItems;
        }
        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {

        }


        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnClearUpload_Click(object sender, RoutedEventArgs e)
        {


        }

        private void clearResult()
        {

        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {

        }


        private void BtnSaveAll_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnUploadMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnCopyMode_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {

        }

        private void FileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }

        private void LoadFileContent(FileItem fileItem)
        {

        }
    }
}
