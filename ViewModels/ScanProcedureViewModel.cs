using Microsoft.Win32;
using ProcedureScanner.Models;
using ProcedureScanner.Services;
using ScanProcedure.Models;
using ScanProcedure.Services;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Documents;

namespace ScanProcedure.ViewModels
{
    public class FileItem
    {
        public string FilePath { get; set; }
        public string FileName { get; set; }
        public string FileContent { get; set; }
        public ScanResult ScanResult { get; set; }
    }
    public class ScanProcedureViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler? PropertyChanged;

        public ScanProcedureViewModel() { }
    }
}