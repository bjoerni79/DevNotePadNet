using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace DevNotePad.Shared.Converter
{
    internal class IsStatusChangedStyleConverter : IValueConverter
    {
        private const string ChangedStyle = "fileStatusChange";

        private const string NotChangedStyle = "fileStatusNoChange";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string resource;

            try
            {
                bool boolToConvert = (bool)value;
                if (boolToConvert)
                {
                    resource = ChangedStyle;
                }
                else
                {
                    resource = NotChangedStyle;
                }
            }
            catch
            {
                resource=NotChangedStyle;
            }

            // Get the style based on the translation above.
            var uiStyle = App.Current.FindResource(resource);
            return uiStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
