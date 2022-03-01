using SSSP;
using SSSP.ProjectValues;
using SSSS.Enums;
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

            var values = project.GlobalProjectValues;
            if (values.ContainsKey(GlobalValueKeys.SiteTitle))
            {
                userSelectedSiteTitle.Text = values[GlobalValueKeys.SiteTitle];
            }
            if (values.ContainsKey(GlobalValueKeys.Author))
            {
                userSelectedAuthorName.Text = values[GlobalValueKeys.Author];
            }
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            var project = (ISimpleStaticSiteProject)DataContext;
            //TODO: so much to do here... Also need to update the interface for the page. :)

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