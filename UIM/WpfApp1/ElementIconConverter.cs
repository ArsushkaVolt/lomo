using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;
using WpfApp1.Models;

namespace WpfApp1

{
   
    public class ElementIconConverter()
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            if (values[0] is ElementType type)
            {
                string iconName = type switch
                {
                    ElementType.Point => "point.png",
                    ElementType.Line => "line.png",
                    ElementType.Circle => "circle.png",
                    ElementType.Ellipse => "ellipse.png",
                    _ => "default.png"
                };

                // Можно использовать pack URI или Embedded resource
                return new BitmapImage(new Uri($"/Resources/Icons/{iconName}", UriKind.Relative));
            }
            return null;

        }
    }

}

   