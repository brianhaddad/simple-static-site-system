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
            var icon = result.Success
                ? MessageBoxImage.Information
                : MessageBoxImage.Error;

            Alert(message, windowTitle, icon);
        }

        public static void ErrorAlert(string message, string windowTitle)
            => Alert(message, windowTitle, MessageBoxImage.Error);

        private static void Alert(string message, string windowTitle, MessageBoxImage icon)
            => MessageBox.Show(message, windowTitle, MessageBoxButton.OK, icon, MessageBoxResult.OK);
    }
}
