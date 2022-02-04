using System;
using System.Collections.Generic;
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

namespace SSSS
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private bool DevMode = false;
        public MainWindow()
        {
            InitializeComponent();
        }

        private void SystemModeButton_Click(object sender, RoutedEventArgs e)
        {
            DevMode = !DevMode;
            if (DevMode)
            {
                SystemModeButton.Content = FindResource("DeveloperMode");
                NewButton.Visibility = Visibility.Visible;
            }
            else
            {
                SystemModeButton.Content = FindResource("DesignerMode");
                NewButton.Visibility = Visibility.Collapsed;
            }
        }
    }
}
