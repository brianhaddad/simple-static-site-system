using SSSP;
using SSSS.Enums;
using System.Windows;
using System.Windows.Navigation;

namespace SSSS.NewSiteProjectWizard
{
    /// <summary>
    /// Interaction logic for NewSiteProjectWizardFinalPage.xaml
    /// </summary>
    public partial class NewSiteProjectWizardFinalPage : PageFunction<WizardResult>
    {
        public NewSiteProjectWizardFinalPage(ISimpleStaticSiteProject project)
        {
            InitializeComponent();

            // Bind wizard state to UI
            DataContext = project;
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
            OnReturn(new ReturnEventArgs<WizardResult>(WizardResult.Finished));
        }
    }
}