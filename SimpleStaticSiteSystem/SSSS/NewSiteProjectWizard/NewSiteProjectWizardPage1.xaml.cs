using SSSP;
using SSSS.Enums;
using System.Windows;
using System.Windows.Navigation;

namespace SSSS.NewSiteProjectWizard
{
    /// <summary>
    /// Interaction logic for NewSiteProjectWizardPage1.xaml
    /// </summary>
    public partial class NewSiteProjectWizardPage1 : PageFunction<WizardResult>
    {
        public NewSiteProjectWizardPage1(ISimpleStaticSiteProject project)
        {
            InitializeComponent();

            // Bind wizard state to UI
            DataContext = project;
        }

        private void nextButton_Click(object sender, RoutedEventArgs e)
        {
            // Go to next wizard page
            //var wizardPage2 = new WizardPage2((ISimpleStaticSiteProject) DataContext);
            //wizardPage2.Return += wizardPage_Return;
            //NavigationService?.Navigate(wizardPage2);
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