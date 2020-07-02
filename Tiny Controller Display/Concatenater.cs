using System;
using System.Globalization;
using System.Windows.Data;

namespace Tiny_Controller_Display {
	public class Concatenater : IValueConverter {
		public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => $"{value}{parameter}";

		public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
	}
}
