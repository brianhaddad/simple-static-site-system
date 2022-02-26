using SSSS.Enums;

namespace SSSS
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