using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ScanProcedure.ViewModels
{
    public partial class MainViewModel : ViewModelBase
    {
        public ObservableCollection<object> Tabs { get; } = new();
        public ScanProcedureViewModel scanProcedureViewModel { get; }

        [ObservableProperty]
        private object? selectedTab;

        public MainViewModel()
        {
            scanProcedureViewModel = new ScanProcedureViewModel();
            Tabs.Add(scanProcedureViewModel);
            selectedTab = scanProcedureViewModel;
        }

    }
}
