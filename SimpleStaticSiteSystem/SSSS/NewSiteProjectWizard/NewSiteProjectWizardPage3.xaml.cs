using SSClasses;
using SSHPW.Extensions;
using SSSP;
using SSSS.Enums;
using SSSS.Helpers;
using System.Windows;
using System.Windows.Navigation;

namespace SSSS
{
    /// <summary>
    /// Interaction logic for NewSiteProjectWizardPage3.xaml
    /// This page is for collecting the project name and picking a location for the file.
    /// </summary>
    public partial class NewSiteProjectWizardPage3 : WizardResultPageFunction
    {
        public NewSiteProjectWizardPage3(ISimpleStaticSiteProject project)
        {
            InitializeComponent();

            // Bind wizard state to UI
            DataContext = project;

            firstPageTitle.Text = "Home";
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            if (firstPageTitle.Text.IsNullEmptyOrWhiteSpace())
            {
                WpfHelpers.ErrorAlert("The First Page Title cannot be blank.", "Validation Error");
                return;
            }

            if (!((bool)isIndex.IsChecked) && (pageFileName.Text.IsNullEmptyOrWhiteSpace() || pageFileName.Text.IsInvalidFileName()))
            {
                //TODO: could be more helpful...
                WpfHelpers.ErrorAlert("If this isn't going to be index.htm you must specify a valid file name.", "Validation Error");
                return;
            }

            var project = (ISimpleStaticSiteProject)DataContext;
            //TODO: update the interface for this page function. :)
            //Should display a list of operations we're about to perform?
            var homePage = new PageDefinition
            {
                IsIndex = (bool)isIndex.IsChecked,
                PageTitle = firstPageTitle.Text,
                FileName = pageFileName.Text,
                NavMenuSortIndex = 0,
                PageLayoutTemplate = "MainLayout.sht",
            };
            var result = project.AddPage(homePage);
            if (!result.Success)
            {
                result.Alert();
                return;
            }

            // Go to next wizard page
            var finalPage = new NewSiteProjectWizardFinalPage(project);
            finalPage.Return += wizardPage_Return;
            NavigationService?.Navigate(finalPage);
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            // Go to previous wizard page
            NavigationService?.GoBack();
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
    }
}