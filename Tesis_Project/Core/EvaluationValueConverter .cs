using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;

namespace Tesis_Project.Core
{
    public class EvaluationValueConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            // Extract the values from the array
            var isLinguisticVisible = (bool)values[0];
            var isIntervalarVisible = (bool)values[1];
            var isRealVisible = (bool)values[2];
            var selectedTerminoLinguistico = values[3]?.ToString();
            var intervalarValue = values[4]?.ToString();
            var realValue = values[5]?.ToString();

            // Return the appropriate value based on visibility
            if (isLinguisticVisible)
                return selectedTerminoLinguistico;
            if (isIntervalarVisible)
                return intervalarValue;
            if (isRealVisible)
                return realValue;

            return string.Empty; // Default value if no condition matches
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
