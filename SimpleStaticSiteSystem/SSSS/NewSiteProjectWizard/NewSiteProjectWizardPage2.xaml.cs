using SSSP;
using SSSP.ProjectValues;
using SSSS.Enums;
using System.Windows;
using System.Windows.Navigation;

namespace SSSS
{
    /// <summary>
    /// Interaction logic for NewSiteProjectWizardPage2.xaml
    /// This page is for collecting the project name and picking a location for the file.
    /// </summary>
    public partial class NewSiteProjectWizardPage2 : WizardResultPageFunction
    {
        public NewSiteProjectWizardPage2(ISimpleStaticSiteProject project)
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
            var test = DataContext as ISimpleStaticSiteProject;
            if (!string.IsNullOrEmpty(userSelectedSiteTitle.Text))
            {
                test.SetGlobalProjectValue(GlobalValueKeys.SiteTitle, userSelectedSiteTitle.Text);
            }
            if (!string.IsNullOrEmpty(userSelectedAuthorName.Text))
            {
                test.SetGlobalProjectValue(GlobalValueKeys.Author, userSelectedAuthorName.Text);
            }
            // Go to next wizard page
            //var wizardPage3 = new WizardPage3((ISimpleStaticSiteProject) DataContext);
            //wizardPage3.Return += wizardPage_Return;
            //NavigationService?.Navigate(wizardPage3);
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