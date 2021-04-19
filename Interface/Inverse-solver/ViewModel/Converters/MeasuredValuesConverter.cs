using Inverse_solver.Model;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Inverse_solver.ViewModel.Converters
{
    public class MeasuredValuesConverter : IValueConverter
    {
        // Take element from VM and convert fror Views
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) { 
        
            StringBuilder result = new StringBuilder("");
            var arr = value as List<Value>;

            if (arr == null) return null;

            for (int i = 0; i < arr.Count; i++)
                result.Append($"{arr[i].X} {arr[i].Y} {arr[i].Z}\n");

            return result.ToString();
        }

        // Take element from Views and convert from VM
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                List<Value> list = new List<Value>();
                string measuredValuesString = value.ToString();
                if (measuredValuesString == "") return list;
                string[] lines = measuredValuesString.Split('\n');
                foreach (var line in lines)
                {
                    // skip empty line
                    if (line == "") continue;

                    // TODO: add error handling
                    double x = 0;
                    double y = 0;
                    double z = 0;

                    string[] values = line.Split(' ');
                    Double.TryParse(values[0], out x);
                    Double.TryParse(values[1], out y);
                    Double.TryParse(values[2], out z);
                    list.Add(new Value(x, y, z));
                }

                return list;
            }
            return null;
        }
    }
}
