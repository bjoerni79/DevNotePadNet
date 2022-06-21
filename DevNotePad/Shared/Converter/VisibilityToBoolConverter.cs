using System;
using System.Windows;
using System.Windows.Data;

namespace DevNotePad.Shared.Converter
{
    internal class VisibilityToBoolConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter,
                      System.Globalization.CultureInfo culture)
        {

            try
            {
                bool boolToConvert = (bool)value;
                if (boolToConvert)
                {
                    return Visibility.Visible;
                }
                else
                {
                    return Visibility.Collapsed;
                }
            }
            catch
            {
                return Visibility.Collapsed;
            }
        }


        public object ConvertBack(object value, Type targetType, object parameter,
                                  System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
