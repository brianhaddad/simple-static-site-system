using System.Windows;

namespace SSSS.Converters
{
    public class BooleanToVisibilityConverter : BooleanConverter<Visibility>
    {
        public BooleanToVisibilityConverter() : base(Visibility.Visible, Visibility.Collapsed) { }
    }
}
