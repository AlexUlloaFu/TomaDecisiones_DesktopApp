using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.Models.MathUtils;

namespace Tesis_Project.Models.Aggregation_Operators
{
    public class MediaPonderada2Tupla 
    {
        public const string ConstName = "Media Ponderada";
        public string Name => ConstName;
        //public Linguistic2Tuple Aggregate(List<Linguistic2Tuple> tuples)
        //{
        //    if (tuples == null || weights == null || tuples.Count != weights.Count)
        //        throw new ArgumentException("La lista de tuplas y la lista de pesos deben tener la misma longitud.");

        //    // Asegurarse de que los pesos están normalizados
        //    float totalWeight = weights.Sum();
        //    if (Math.Abs(totalWeight - 1.0f) > 0.0001f)
        //        throw new ArgumentException("Los pesos deben estar normalizados y sumar 1.");

        //    // Calcular el valor ponderado agregado
        //    float weightedSum = 0;
        //    for (int i = 0; i < tuples.Count; i++)
        //    {
        //        weightedSum += weights[i] * (tuples[i].IndexTermino + tuples[i].SimbolicTraslation);
        //    }

        //    // Determinar el índice lingüístico más cercano
        //    int k = (int)Math.Round(weightedSum);

        //    // Calcular la traslación simbólica corregida
        //    float deltaK = weightedSum - k;

        //    // Crear y devolver la 2-tupla agregada
        //    return new Linguistic2Tuple
        //    {
        //        IndexTermino = k,
        //        SimbolicTraslation = deltaK
        //    };

        //}

    }
}
