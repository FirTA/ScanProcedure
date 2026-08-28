using Microsoft.Win32;
using ProcedureScanner.Models;
using ProcedureScanner.Services;
using ScanProcedure.Models;
using System.Collections.ObjectModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Reflection;
using System.Security.Policy;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using Sh = System.Windows.Shapes;

namespace ScanProcedure
{
    /// <summary>
    /// Represents a file item in the navigation panel
    /// </summary>
    public class FileItem
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public ScanResult ScanResult { get; set; }
    }

    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScanResult _lastResult;
        private ObservableCollection<FileItem> _fileItems = new ObservableCollection<FileItem>();
        private FileItem _currentSelectedFile;

        public MainWindow()
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
            if (DbPicker.SelectedItem == null)
            {
                MessageBox.Show("Select DB Type First!");
                return;
            }

            if (_currentSelectedFile == null)
            {
                MessageBox.Show("No file selected!");
                return;
            }

            var sql = _currentSelectedFile.FileContent;

            var dbType1 = (Item)DbPicker.SelectedValue;
            if (dbType1.Name == DatabaseType.Oracle.ToString())
            {
                _lastResult = OracleRegexScanner.ParseScript(sql);
                _currentSelectedFile.ScanResult = _lastResult;
            }
            else if (dbType1.Name == DatabaseType.PostgreSQL.ToString())
            {
                _lastResult = PostgresRegexScanner.ParseScript(sql);
                _currentSelectedFile.ScanResult = _lastResult;
            }
            else
            {
                MessageBox.Show("Unsupported database type!");
                return;
            }

            resultText.Text = _lastResult != null ? _lastResult.ToMarkdown() : "";
            ReadTable.Content = _lastResult.ReadTables.Count;
            WriteTable.Content = _lastResult.WriteTables.Count;
            Calls.Content = _lastResult.Procedures.Count;
        }
       

        private void BtnCopy_Click(object sender, RoutedEventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(resultText.Text))
            {
                Clipboard.SetText(resultText.Text.ToString());
            }
        }

        private void BtnSave_Click(object sender, RoutedEventArgs e)
        {
            if (_lastResult == null)
            {
                MessageBox.Show("No result to save. Please process a file first.");
                return;
            }

            var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ScanProcedure", _lastResult.ProcedureName + ".md");
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.WriteAllText(filename, _lastResult.ToMarkdown().ToString());
            MessageBox.Show($"File saved to: {filename}");
        }

        
        private void BtnSaveAll_Click(object sender, RoutedEventArgs e)
        {
            if (_lastResult == null)
            {
                MessageBox.Show("No result to save. Please process a file first.");
                return;
            }

            foreach( var item in _fileItems)
            {
                var filename = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ScanProcedure", item.FileName + ".md");
                Directory.CreateDirectory(Path.GetDirectoryName(filename));
                File.WriteAllText(filename, item.ScanResult.ToMarkdown().ToString());

            }

            MessageBox.Show($"File saved to MyDocument/ScanProcedure");
        }

        private void BtnUploadMode_Click(object sender, RoutedEventArgs e)
        {
            RightPanel.Visibility = Visibility.Collapsed;
            LeftPanel.Visibility = Visibility.Collapsed;
            BtnUploadMode.Visibility = Visibility.Collapsed;

            UploadContainer.Visibility = Visibility.Visible;
            BtnCopyMode.Visibility = Visibility.Visible;
            BtnSaveAll.Visibility = Visibility.Visible;

            // Clear navigation when entering upload mode
            NavPanel.Visibility = Visibility.Collapsed;
            ColNav.Width = new GridLength(0);
        }

        private void BtnCopyMode_Click(object sender, RoutedEventArgs e)
        {
            BtnProcess.Visibility = Visibility.Visible;
            RightPanel.Visibility = Visibility.Visible;
            LeftPanel.Visibility = Visibility.Visible;
            BtnUploadMode.Visibility = Visibility.Visible;

            UploadContainer.Visibility = Visibility.Collapsed;
            BtnCopyMode.Visibility = Visibility.Collapsed;

            // Show navigation if there are files
            if (_fileItems.Count > 0)
            {
                NavPanel.Visibility = Visibility.Visible;
                ColNav.Width = new GridLength(150, GridUnitType.Pixel);
            }
        }

        private void BtnSelectFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog
            {
                Multiselect = true,
                Filter = "SQL Files (*.sql)|*.sql|Text Files (*.txt)|*.txt|All Files (*.*)|*.*"
            };

            if (openFileDialog.ShowDialog() == true)
            {
                // Clear existing files
                _fileItems.Clear();

                // Load all selected files
                foreach (string filePath in openFileDialog.FileNames)
                {
                    string fileContent = File.ReadAllText(filePath);
                    var fileItem = new FileItem
                    {
                        FilePath = filePath,
                        FileName = Path.GetFileName(filePath),
                        FileContent = fileContent
                    };
                    _fileItems.Add(fileItem);
                }

                // Show navigation panel
                ColNav.Width = new GridLength(150, GridUnitType.Pixel);
                NavPanel.Visibility = Visibility.Visible;

                // Hide upload container and restore panels
                UploadContainer.Visibility = Visibility.Collapsed;
                LeftPanel.Visibility = Visibility.Visible;
                RightPanel.Visibility = Visibility.Visible;
                BtnProcess.Visibility = Visibility.Collapsed;

                // Select the first item by default
                if (_fileItems.Count > 0)
                {
                    FileListBox.SelectedItem = _fileItems[0];
                    LoadFileContent(_fileItems[0]);
                }
            }
        }

        private void FileListBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (FileListBox.SelectedItem is FileItem selectedFile)
            {
                LoadFileContent(selectedFile);
            }
        }

        private void LoadFileContent(FileItem fileItem)
        {
            _currentSelectedFile = fileItem;

            // Load the file content into the RichTextBox
            ScriptSQL.Document.Blocks.Clear();
            ScriptSQL.Document.Blocks.Add(new Paragraph(new Run(fileItem.FileContent)));

            // If there's a stored scan result, display it
            if (fileItem.ScanResult != null)
            {
                resultText.Text = fileItem.ScanResult.ToMarkdown();
                ReadTable.Content = fileItem.ScanResult.ReadTables.Count;
                WriteTable.Content = fileItem.ScanResult.WriteTables.Count;
                Calls.Content = fileItem.ScanResult.Procedures.Count;
                _lastResult = fileItem.ScanResult;
            }
            else
            {
                // Clear results if no scan exists
                resultText.Text = "File loaded. Click 'Process' to scan.";
                ReadTable.Content = "0";
                WriteTable.Content = "0";
                Calls.Content = "0";
                _lastResult = null;
            }
        }

    }
}