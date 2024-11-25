using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.Models.MathUtils;

namespace Tesis_Project.Models.Aggregation_Operators
{
    public class MediaAritmetica2Tupla : IAggregationOperator
    {
        public const string ConstName = "Media Aritmética";
        public string Name => ConstName;
        public Linguistic2Tuple Aggregate(List<Linguistic2Tuple> tuples)
        {
            if (tuples == null || tuples.Count == 0)
                throw new ArgumentException("The list of tuples cannot be null or empty.");

            // Paso 1: Sumatoria de cada 2-tupla en su valor numérico
            double sum = 0;
            foreach (var tuple in tuples)
            {
                sum += tuple.IndexTermino + tuple.SimbolicTraslation;
            }

            // Paso 2: Calcular la media aritmética de los valores numéricos
            double meanValue = sum / tuples.Count;

            // Paso 3: Convertir el valor medio de vuelta a una 2-tupla lingüística
            int meanTermIndex = (int)Math.Floor(meanValue); // Parte entera, como índice del término
            double meanAlpha = meanValue - meanTermIndex; // Residuo, como desplazamiento
            
            if(meanAlpha >= 0.5)
            {
                meanTermIndex++;
                meanAlpha = meanAlpha - 1;
            }

            if (meanAlpha < -0.5)
            {
                meanTermIndex--;
                meanAlpha = 1 - meanAlpha;
            }

            meanAlpha = Math.Round(meanAlpha, 2);

            // Ajustar el índice si sale fuera del rango de términos válidos
            if (meanTermIndex < 0)
            {
                meanTermIndex = 0;
                meanAlpha = 0;
            }
            else if (meanTermIndex >= ComputacionalModel.Instance.CBTL.TerminosLinguisticos.Count)
            {
                meanTermIndex = ComputacionalModel.Instance.CBTL.TerminosLinguisticos.Count - 1;
                meanAlpha = 0;
            }

            // Devolver la media como una nueva 2-tupla
            return new Linguistic2Tuple(meanTermIndex, (float)meanAlpha);

        }

    }
}
