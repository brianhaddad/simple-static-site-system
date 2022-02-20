using SSSS.Enums;

namespace SSSS.NewSiteProjectWizard
{
    public class NewSiteProjectWizardReturnEventArgs
    {
        public WizardResult Result { get; }
        public object Data { get; }

        public NewSiteProjectWizardReturnEventArgs(WizardResult result, object data)
        {
            Result = result;
            Data = data;
        }
    }
}