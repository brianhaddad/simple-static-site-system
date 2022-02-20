using SSSP;
using SSSS.Enums;
using System;
using System.Windows.Navigation;

namespace SSSS.NewSiteProjectWizard
{
    public class NewSiteProjectWizardLauncher : PageFunction<ISimpleStaticSiteProject>
    {
        private readonly ISimpleStaticSiteProject _project;
        public event NewSiteProjectWizardReturnEventHandler WizardReturn;

        public NewSiteProjectWizardLauncher(ISimpleStaticSiteProject project)
        {
            _project = project ?? throw new ArgumentNullException(nameof(project));
        }

        protected override void Start()
        {
            base.Start();

            // So we remember the WizardCompleted event registration
            KeepAlive = true;

            // Launch the wizard
            var wizardPage1 = new NewSiteProjectWizardPage1(_project);
            wizardPage1.Return += wizardPage_Return;
            NavigationService?.Navigate(wizardPage1);
        }

        public void wizardPage_Return(object sender, ReturnEventArgs<WizardResult> e)
        {
            // Notify client that wizard has completed
            // NOTE: We need this custom event because the Return event cannot be
            // registered by window code - if WizardDialogBox registers an event handler with
            // the WizardLauncher's Return event, the event is not raised.
            WizardReturn?.Invoke(this, new NewSiteProjectWizardReturnEventArgs(e.Result, _project));
            OnReturn(null);
        }
    }
}
