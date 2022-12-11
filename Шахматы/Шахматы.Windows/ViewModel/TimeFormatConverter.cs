using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;

namespace Шахматы.ViewModel
{
    class TimeFormatConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if(value is int)
            {
                if ((int)value == -1)
                    return "";
                int seconds = (int)value % 60;
                int minutes = ((int)value - seconds) / 60;
                int hours = (minutes - minutes % 60) / 60;
                minutes = minutes - (hours * 60);
                string h = hours.ToString() + " : ";
                if (hours < 10)
                    h = "0"+hours+" : ";
                string m = minutes.ToString() + " : ";
                if (minutes < 10)
                    m = "0" + minutes + " : ";
                string sec = seconds.ToString();
                if (seconds < 10)
                    sec = "0" + seconds;
                string s = h + m + sec;
                return s;
            }
            return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
}
