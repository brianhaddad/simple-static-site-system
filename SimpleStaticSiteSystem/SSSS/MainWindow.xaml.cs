using SSHFH;
using SSHFH.Tools;
using SSHPW;
using SSSP;
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
        private readonly ISimpleStaticSiteProject SiteProject;
        private bool DevMode = false;
        public MainWindow()
        {
            InitializeComponent();
            SiteProject = new SimpleStaticSiteProject(new SuperSimpleHtmlFileHandler(new FileIo(), new SuperSimpleHtmlParserWriter()));
            EvaluateButtonStates();
            EvaluateDevMode();
        }

        private void SystemModeButton_Click(object sender, RoutedEventArgs e)
        {
            DevMode = !DevMode;
            EvaluateDevMode();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            //actually need a wizard or popup to fill in this data...
            //var result = SiteProject.CreateNew("", "", false);
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EvaluateButtonStates()
        {
            SaveButton.IsEnabled = SiteProject.UnsavedChanges;
        }

        private void EvaluateDevMode()
        {
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
