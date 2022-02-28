using SSSP;
using SSSP.Classes;
using SSSS.Enums;
using SSSS.Helpers;
using System;
using System.IO;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Navigation;

namespace SSSS
{
    /// <summary>
    /// Interaction logic for NewSiteProjectWizardPage1.xaml
    /// This page is for collecting the project name and picking a location for the file.
    /// </summary>
    public partial class NewSiteProjectWizardPage1 : WizardResultPageFunction
    {
        private string lastUserSelectedFolderLocation = "";
        private string lastUserSelectedFileName = "";

        public NewSiteProjectWizardPage1(ISimpleStaticSiteProject project)
        {
            InitializeComponent();

            project.UserSelectedFolderLocation =
                Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "SimpleStaticSites");
            project.UserSelectedFileName = "NewProject";

            // Bind wizard state to UI
            DataContext = project;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            // Check for valid data:
            var project = (ISimpleStaticSiteProject)DataContext;

            var dataChanged = lastUserSelectedFileName != project.UserSelectedFileName
                || lastUserSelectedFolderLocation != project.UserSelectedFolderLocation;

            var result = dataChanged
                ? project.CreateNew(project.UserSelectedFolderLocation, project.UserSelectedFileName, true)
                : FileActionResult.Successful();

            if (Path.IsPathFullyQualified(userSelectedFolderLocation.Text) && result.Success)
            {
                lastUserSelectedFolderLocation = project.UserSelectedFolderLocation;
                lastUserSelectedFileName = project.UserSelectedFileName;

                // Go to next wizard page
                var wizardPage2 = new NewSiteProjectWizardPage2(project);
                wizardPage2.Return += wizardPage_Return;
                NavigationService?.Navigate(wizardPage2);
            }
            else
            {
                userSelectedFolderLocation.Focus();
                if (!result.Success)
                {
                    result.Alert();
                }
            }
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Cancel the wizard and don't return any data
            OnReturn(new ReturnEventArgs<WizardResult>(WizardResult.Canceled));
        }

        public void wizardPage_Return(object sender, ReturnEventArgs<WizardResult> e)
        {
            // If returning, wizard was completed (finished or canceled),
            // so continue returning to calling page
            OnReturn(e);
        }

        private void browseFolder_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new FolderBrowserDialog();
            dialog.SelectedPath = userSelectedFolderLocation.Text;
            var result = dialog.ShowDialog();
            if (result == DialogResult.OK)
            {
                userSelectedFolderLocation.Text = dialog.SelectedPath;
            }
        }
    }
}