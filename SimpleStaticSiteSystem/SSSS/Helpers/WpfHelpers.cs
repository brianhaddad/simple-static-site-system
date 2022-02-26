using SSSP.Classes;
using System.Windows;

namespace SSSS.Helpers
{
    public static class WpfHelpers
    {
        public static void Alert(this FileActionResult result)
        {
            var message = !string.IsNullOrEmpty(result.Message)
                ? result.Message
                : result.Success
                    ? "Success!"
                    : "Failure, but we don't know why!";
            var windowTitle = "File Action Result Message";
            var button = MessageBoxButton.OK;
            var icon = result.Success
                ? MessageBoxImage.Information
                : MessageBoxImage.Error;

            MessageBox.Show(message, windowTitle, button, icon, MessageBoxResult.Yes);
        }
    }
}
