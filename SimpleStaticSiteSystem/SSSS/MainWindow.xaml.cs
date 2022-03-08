using SSHFH;
using SSHFH.Tools;
using SSHPW;
using SSSP;
using SSSS;
using SSSS.Helpers;
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
            EvaluateEnabledStates();
            EvaluateProjectModes();
        }

        private void SystemModeButton_Click(object sender, RoutedEventArgs e)
        {
            DevMode = !DevMode;
            EvaluateProjectModes();
        }

        private void NewButton_Click(object sender, RoutedEventArgs e)
        {
            //actually need a wizard or popup to fill in this data...
            //var result = SiteProject.CreateNew("", "", false);
            var wizard = new NewSiteProjectWizardDialogBox(SiteProject);
            var showDialog = wizard.ShowDialog();
            var dialogResult = showDialog is not null && (bool)showDialog;
            if (dialogResult)
            {
                SiteProject.Save();
            }
            EvaluateProjectModes();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {

            EvaluateProjectModes();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SiteProject.UnsavedChanges)
            {
                var result = SiteProject.Save();
                if (!result.Success)
                {
                    result.Alert();
                }
            }
        }

        private void BuildProject_Click(object sender, RoutedEventArgs e)
        {

        }

        private void EvaluateEnabledStates()
        {
            SaveButton.IsEnabled = SiteProject.UnsavedChanges;
        }

        private void EvaluateProjectModes()
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

            if (SiteProject.ValidProjectLoaded)
            {
                LoadedProjectToolbar.Visibility = Visibility.Visible;
            }
            else
            {
                LoadedProjectToolbar.Visibility = Visibility.Collapsed;
            }
        }
    }
}
