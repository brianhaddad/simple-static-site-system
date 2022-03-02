using SSSP;
using SSSS.Enums;
using SSSS.Helpers;
using System.Windows;
using System.Windows.Navigation;

namespace SSSS
{
    /// <summary>
    /// Interaction logic for NewSiteProjectWizardFinalPage.xaml
    /// </summary>
    public partial class NewSiteProjectWizardFinalPage : WizardResultPageFunction
    {
        public NewSiteProjectWizardFinalPage(ISimpleStaticSiteProject project)
        {
            InitializeComponent();

            // Bind wizard state to UI
            DataContext = project;
            //TODO: display the actions that will be taken before the user clicks FINISH.
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            // Cancel the wizard and don't return any data
            OnReturn(new ReturnEventArgs<WizardResult>(WizardResult.Canceled));
        }

        private void backButton_Click(object sender, RoutedEventArgs e)
        {
            // Go to previous wizard page
            NavigationService?.GoBack();
        }

        private void finishButton_Click(object sender, RoutedEventArgs e)
        {
            // Finish the wizard and return bound data to calling page
            var project = (ISimpleStaticSiteProject)DataContext;
            var result = project.Save();
            if (!result.Success)
            {
                result.Alert();
                return;
            }
            OnReturn(new ReturnEventArgs<WizardResult>(WizardResult.Finished));
        }
    }
}