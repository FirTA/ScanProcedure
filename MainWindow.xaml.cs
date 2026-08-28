using Microsoft.Win32;
using ProcedureScanner.Models;
using ProcedureScanner.Services;
using ScanProcedure.Models;
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
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private ScanResult _lastResult;
        public MainWindow()
        {
            InitializeComponent();
            var items = Enum.GetNames<DatabaseType>().Select(name => new Item{ Name = name }).ToList();
            DbPicker.ItemsSource = items;
            DbPicker.SelectedItem = items[0];
        }

        private void BtnProcess_Click(object sender, RoutedEventArgs e)
        {
            if (DbPicker.SelectedItem == null)
            {
                MessageBox.Show("Select DB Type First!");
                return;
            }

            var doc = new TextRange(ScriptSQL.Document.ContentStart, ScriptSQL.Document.ContentEnd);
            var sql = doc.Text;

            var dbType = DbPicker.SelectedIndex.ToString();
            var dbType1 = (Item)DbPicker.SelectedValue;
            if (dbType1.Name == DatabaseType.Oracle.ToString())
            {
                _lastResult = OracleRegexScanner.ParseScript(sql);
            }
            if (dbType1.Name == DatabaseType.PostgreSQL.ToString())
            {
                _lastResult = PostgresRegexScanner.ParseScript(sql);
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
            var filename = Path.Combine( Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),"ScanProcedure",_lastResult.ProcedureName+".md");
            Directory.CreateDirectory(Path.GetDirectoryName(filename));
            File.WriteAllText(filename, _lastResult.ToMarkdown().ToString());
        }

        private void BtnUploadMode_Click(object sender, RoutedEventArgs e)
        {
            RightPanel.Visibility = Visibility.Collapsed;
            LeftPanel.Visibility = Visibility.Collapsed;
            BtnUploadMode.Visibility = Visibility.Collapsed;

            UploadContainer.Visibility = Visibility.Visible;
            BtnCopyMode.Visibility = Visibility.Visible;
        }

        private void BtnCopyMode_Click(object sender, RoutedEventArgs e)
        {
            RightPanel.Visibility = Visibility.Visible;
            LeftPanel.Visibility = Visibility.Visible;
            BtnUploadMode.Visibility = Visibility.Visible;

            UploadContainer.Visibility = Visibility.Collapsed;
            BtnCopyMode.Visibility = Visibility.Collapsed;
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
                string fileContent = File.ReadAllText(openFileDialog.FileName);
                ScriptSQL.Document.Blocks.Clear();
                ScriptSQL.Document.Blocks.Add(new Paragraph(new Run(fileContent)));


                // Hide the upload placeholder
                UploadContainer.Visibility = Visibility.Collapsed;

                // Restore Left & Right panels
                LeftPanel.Visibility = Visibility.Visible;
                RightPanel.Visibility = Visibility.Visible;

                if (openFileDialog.FileNames.Count() > 1)
                {
                    ColNav.Width = new GridLength(150, GridUnitType.Pixel);
                    NavPanel.Visibility = Visibility.Visible;
                }

            }
        }
    }
}