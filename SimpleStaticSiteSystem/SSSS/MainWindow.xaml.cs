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
            var wizard = new NewSiteProjectWizardDialogBox(SiteProject);
            var showDialog = wizard.ShowDialog();
            var dialogResult = showDialog is not null && (bool)showDialog;
            if (dialogResult)
            {
                // This Save() is superfluous. Not sure what I want to do...
                SiteProject.Save();
            }
            EvaluateInterface();
        }

        private void OpenButton_Click(object sender, RoutedEventArgs e)
        {
            //TODO: I really need to be able to open existing projects...
            EvaluateProjectModes();
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            if (SiteProject.UnsavedChanges)
            {
                var result = SiteProject.Save();
                if (!result.Success)
                {
                    result.Alert("Save Failed");
                }
            }
        }

        private void BuildProject_Click(object sender, RoutedEventArgs e)
        {
            var buildDefinition = BuildTargetSelection.SelectedValue as string;
            if (buildDefinition is not null)
            {
                var result = SiteProject.Build(buildDefinition);
                if (!result.Success)
                {
                    result.Alert("Build Failed");
                }
            }
        }

        private void EvaluateInterface()
        {
            EvaluateEnabledStates();
            EvaluateProjectModes();
            PopulateDropdowns();
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

        private void PopulateDropdowns()
        {
            // BuildTargetSelection
            BuildTargetSelection.Items.Clear();
            var buildTargets = SiteProject.ProjectBuildTargetDefinitions;
            foreach (var buildTarget in buildTargets)
            {
                BuildTargetSelection.Items.Add(buildTarget.Key);
            }
            BuildTargetSelection.SelectedIndex = 0;
        }
    }
}
