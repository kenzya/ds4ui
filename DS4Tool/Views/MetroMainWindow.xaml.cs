using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

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
            options.IsOpen = false;
            logs.IsOpen = false;

            if (about.IsOpen)
                about.IsOpen = false;
            else
                about.IsOpen = true;
        }

        private void Options_Click(object sender, RoutedEventArgs e)
        {
            about.IsOpen = false;
            logs.IsOpen = false;

            if (options.IsOpen)
                options.IsOpen = false;
            else
                options.IsOpen = true;
        }

        private void Logs_Click(object sender, RoutedEventArgs e)
        {
            about.IsOpen = false;
            options.IsOpen = false;

            if (logs.IsOpen)
                logs.IsOpen = false;
            else
                logs.IsOpen = true;
        }
    }
}
