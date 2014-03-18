using System.Windows;
using MahApps.Metro.Controls;

namespace DS4Tool
{
    /// <summary>
    /// Logica di interazione per MainWindow.xaml
    /// </summary>
    public partial class MetroMainWindow : MetroWindow
    {
        public MetroMainWindow()
        {
            InitializeComponent();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            about.IsOpen = true;
        }
    }
}
