using SSSP;
using SSSS.Enums;
using System.Windows.Navigation;

namespace SSSS.NewSiteProjectWizard
{
    /// <summary>
    /// Interaction logic for NewSiteProjectWizardDialogBox.xaml
    /// </summary>
    public partial class NewSiteProjectWizardDialogBox : NavigationWindow
    {
        public NewSiteProjectWizardDialogBox(ISimpleStaticSiteProject project)
        {
            InitializeComponent();

            var wizardLauncher = new NewSiteProjectWizardLauncher(project);
            wizardLauncher.WizardReturn += wizardLauncher_WizardReturn;
            Navigate(wizardLauncher);
        }

        private void wizardLauncher_WizardReturn(object sender, NewSiteProjectWizardReturnEventArgs e)
        {
            // Handle wizard return
            if (DialogResult is null)
            {
                DialogResult = (e.Result == WizardResult.Finished);
            }
        }
    }
}
