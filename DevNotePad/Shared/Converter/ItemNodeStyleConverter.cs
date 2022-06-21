using DevNotePad.Features.Shared;
using System;
using System.Globalization;
using System.Windows.Data;

namespace DevNotePad.Shared.Converter
{
    internal class ItemNodeStyleConverter : IValueConverter
    {
        private const string Default = "itemNodeStyleDefault";
        private const string Group = "itemNodeStyleGroup";
        private const string Title = "itemNodeStyleTitle";
        private const string Element = "itemNodeStyleElement";
        private const string Attribute = "itemNodeStyleAttribute";
        private const string Value = "itemNodeStyleValue";

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string selectUiStyle = Default;
            if (value.GetType() == typeof(ItemNodeStyle))
            {
                var style = (ItemNodeStyle)value;
                switch (style)
                {
                    case ItemNodeStyle.Group:
                        selectUiStyle = Group;
                        break;
                    case ItemNodeStyle.Title:
                        selectUiStyle = Title;
                        break;
                    case ItemNodeStyle.Element:
                        selectUiStyle = Element;
                        break;
                    case ItemNodeStyle.Attribute:
                        selectUiStyle = Attribute;
                        break;
                    case ItemNodeStyle.Value:
                        selectUiStyle = Value;
                        break;
                    default:
                        selectUiStyle = Default;
                        break;
                }
            }

            // Get the style based on the translation above.
            var uiStyle = App.Current.FindResource(selectUiStyle);
            return uiStyle;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
