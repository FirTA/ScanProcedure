
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace ScanProcedure.Views
{
    /// <summary>
    /// Interaction logic for TitleBar.xaml
    /// </summary>
    public partial class TitleBar : UserControl
    {
        public TitleBar()
        {
            InitializeComponent();
        }
        private void TitleBar_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed)
            {
                Window.GetWindow(this)?.DragMove();
            }
        }

        private void Minimize_Click(object sender, RoutedEventArgs e) => Window.GetWindow(this).WindowState = WindowState.Minimized;

        private void Maximize_Click(object sender, RoutedEventArgs e)
        {
            Window.GetWindow(this).WindowState = WindowState.Maximized;
            NormalWindow.Visibility = Visibility.Visible;
            MaxWindow.Visibility = Visibility.Collapsed;

        }
        private void Normal_Click(object sender, RoutedEventArgs e) 
        { 
            Window.GetWindow(this).WindowState = WindowState.Normal;
            NormalWindow.Visibility = Visibility.Collapsed;
            MaxWindow.Visibility = Visibility.Visible;
        } 
        private void Close_Click(object sender, RoutedEventArgs e) => Window.GetWindow(this).Close();
    }
}
