using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace DevNotePad.Shared.Converter
{
    internal class RegularExpressionResultConverter : IValueConverter
    {
        private readonly string path = @"/Shared/Image/";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            RegularExpressionResult result = (RegularExpressionResult)value;

            if (result == RegularExpressionResult.Match || result == RegularExpressionResult.NoMatch)
            {
                string uriString;
                if (result == RegularExpressionResult.Match)
                {
                    uriString = path + "OK.png";
                }
                else
                {
                    uriString = path + "NotOK.png";
                }

                var uri = new Uri(uriString, UriKind.Relative);
                var imageStream = Application.GetResourceStream(uri);

                // https://docs.microsoft.com/en-us/dotnet/api/system.windows.media.imaging.bitmapdecoder?view=netcore-3.1
                BitmapDecoder decoder = BitmapDecoder.Create(imageStream.Stream, BitmapCreateOptions.None, BitmapCacheOption.Default);

                imageStream.Stream.Close();
                return decoder.Frames[0];
            }
            else
            {
                // Nothing to do. Empty state.
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
