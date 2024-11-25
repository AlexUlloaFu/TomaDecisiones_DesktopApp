using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.Models.Domains;
using Tesis_Project.Models.MathUtils;

namespace Tesis_Project.Models
{
    class ComputacionalModel
    {
        private static ComputacionalModel _instance;
        public static ComputacionalModel Instance
        {
            get
            {
                if (_instance == null) _instance = new ComputacionalModel();
                return _instance;
            }
        }

        private MarcoDecisionModel marcoDecision = MarcoDecisionModel.Instance;
        public DomainModel CBTL = new DomainModel();
        public IAggregationOperator aggregationOperator;
        public Collection<UnifiedPreference> UnifiedPreferences = new Collection<UnifiedPreference>();

        public ComputacionalModel()
        {
        }

        public ComputacionalModel(MarcoDecisionModel marcoDecision)
        {
            this.marcoDecision = marcoDecision;
        }

        // Metodo que dado un conjunto de terminos lingsuiticos devuelve su representacion grafica en funciones tringulares
        public Collection<TriangularFunction> CreateTriangularFunctions(List<string> terms, Collection<TriangularFunction> oldFunctions)
        {
            Collection<TriangularFunction> funcionesPertenencia = new Collection<TriangularFunction>();
            double step = 1.0 / (oldFunctions.Count);
            for (int i = 0; i <= oldFunctions.Count; i++)
            {
                double left = i == 0 ? 0 : (i - 1) * step;
                double peak = i == oldFunctions.Count ? 1 : i * step;
                double right = i == oldFunctions.Count ? 1 : (i + 1) * step;
                funcionesPertenencia.Add(new TriangularFunction(
                terminoName: terms[i],
                leftPoint: new Point(left, i == 0 ? 1 : 0),
                peakPoint: new Point(peak, 1),
                rightPoint: new Point(right, i == oldFunctions.Count ? 1 : 0)
                ));
            }

            return funcionesPertenencia;
        }

        public UnifiedPreference UnifyPreference(PreferenceModel preference)
        {
            UnifiedPreference unifiedPreference = new UnifiedPreference { expert = preference.expert, alternative = preference.alternative, criteria = preference.criteria };

            string closestTerm = "";
            double delta = 0;

            if (preference.Domain.Type == DomainType.Real)
            {
                unifiedPreference.RealValue = preference.RealValue;
                closestTerm = UnifyRealToLinguisticDomain(preference.RealValue/100, CBTL);
            }
            if(preference.Domain.Type == DomainType.Intervalar)
            {
                unifiedPreference.LowerLimit = preference.LowerLimit;
                unifiedPreference.UpperLimit = preference.UpperLimit;

                var fuzzySet = UnifyIntervalToLinguisticDomain(preference.LowerLimit , preference.UpperLimit, CBTL);
                // 2. Encontrar el término con mayor pertenencia en el dominio destino
                double maxMembershipValue = 0;

                foreach (var fuzzyValue in fuzzySet)
                {
                    if (fuzzyValue.Value > maxMembershipValue)
                    {
                        closestTerm = fuzzyValue.Key;
                        maxMembershipValue = fuzzyValue.Value;
                    }
                }

                // 4. Calcular el error de redondeo (Delta)
                delta = 1 - maxMembershipValue;
            }
            if(preference.Domain.Type == DomainType.Linguistico)
            {
                unifiedPreference.SelectedTerminoLinguistico = preference.SelectedTerminoLinguistico;

                // 1. Obtener el conjunto difuso transformado al dominio objetivo
                var fuzzySet = UnifyLinguisticToLinguisticDomain(preference.SelectedTerminoLinguistico, preference.Domain, CBTL);

                // 2. Encontrar el término con mayor pertenencia en el dominio destino
                double maxMembershipValue = 0;

                foreach (var fuzzyValue in fuzzySet)
                {
                    if (fuzzyValue.Value > maxMembershipValue)
                    {
                        closestTerm = fuzzyValue.Key;
                        maxMembershipValue = fuzzyValue.Value;
                    }
                }

                // 4. Calcular el error de redondeo (Delta)
                delta = 1 - maxMembershipValue;
            }
          
            int closestTermIndex = CBTL.TerminosLinguisticos.IndexOf(closestTerm);

            // 5. Crear la 2-tupla
            if (delta >= 0.5)
            {
                closestTermIndex++;
                delta = delta - 1;
            }

            if (delta < -0.5)
            {
                closestTermIndex--;
                delta = 1 - delta;
            }

            delta = Math.Round(delta, 2);

            Linguistic2Tuple tuple = new Linguistic2Tuple(closestTermIndex, delta);

            unifiedPreference.Domain = CBTL;
            unifiedPreference.IndexTermino = closestTermIndex;
            unifiedPreference.NewTerminoLinguistico = CBTL.TerminosLinguisticos[closestTermIndex];
            unifiedPreference.Tuple = tuple;

            return unifiedPreference;
        }

        //public Linguistic2Tuple TransformPreferenceTo2Tuple(PreferenceModel preference)
        //{

        //}

        // Metodo que recibe un conjunto de preferencias y las devuelve en 2 tupla linguistica
        public List<Linguistic2Tuple> Add2TuplesToPreferences(List<UnifiedPreference> preferences)
        {
            List<Linguistic2Tuple> prefenecesIn2Tuples = new List<Linguistic2Tuple>();

            foreach (PreferenceModel preference in preferences)
            {
                Linguistic2Tuple tuple = new Linguistic2Tuple(preference.IndexTermino, 0);
                prefenecesIn2Tuples.Add(tuple);
            }

            return prefenecesIn2Tuples;
        }

        // Metodo que realiza la agregacion de preferencias
        
        public IOrderedEnumerable<KeyValuePair<string, Linguistic2Tuple>> AggregatePreferences()
        {
            if (marcoDecision.Preferences == null) return null;

            List<UnifiedPreference> generalPreferences = UnifiedPreferences.ToList();

            //Agrupar preferencias por criterio
            Dictionary<string,List<UnifiedPreference>> preferencesByCriteria = generalPreferences
           .GroupBy(preference => preference.criteria.ID)
           .ToDictionary(group => group.Key, group => group.ToList());

            //Agrupar por alternativa cada preferencia colectiva de cada criterio
            Dictionary<string, List<Linguistic2Tuple>> aggregatedResultsByAlternative = new Dictionary<string, List<Linguistic2Tuple>>();

            foreach (var criteriaGroup in preferencesByCriteria)
            {
                string criteriaId = criteriaGroup.Key;
                // Agrupar preferencias de un criterio por alternativa
                Dictionary<string, List<UnifiedPreference>> preferencesForAlternative = criteriaGroup.Value
                    .GroupBy(preference => preference.alternative.ID)
                    .ToDictionary(group => group.Key, group => group.ToList());

                foreach (var alternativeGroup in preferencesForAlternative)
                {
                    string alternativeID = alternativeGroup.Key;
                    List<UnifiedPreference> preferencesOfAlternative = alternativeGroup.Value;
                    List<Linguistic2Tuple> preferencesIn2Tuples = Add2TuplesToPreferences(preferencesOfAlternative);
                    //Obtener los pesos de los expertos involucrados en esta alternativa
                    List<double> expertWeights = GetWeightsOfPreferences(preferencesOfAlternative, "expert");
                    Linguistic2Tuple aggregatedTuple = aggregationOperator.Aggregate(preferencesIn2Tuples);

                    // Add the aggregated tuple to the list for this alternative
                    if (!aggregatedResultsByAlternative.ContainsKey(alternativeID))
                        aggregatedResultsByAlternative[alternativeID] = new List<Linguistic2Tuple>();
                    
                    aggregatedResultsByAlternative[alternativeID].Add(aggregatedTuple);
                }
            }

            // Final aggregated results for each alternative
            Dictionary<string, Linguistic2Tuple> aggregatedResults = new Dictionary<string, Linguistic2Tuple>();

            foreach ( var alternativeGroup in aggregatedResultsByAlternative)
            {
                string alternativeID = alternativeGroup.Key;
                List<Linguistic2Tuple> aggregatedCriterias = alternativeGroup.Value;
                //Obtener los pesos de los expertos involucrados en esta alternativa
                //List<double> criteriaWeights = GetWeightsOfPreferences(aggregatedCriterias, "criteria");

                aggregatedResults[alternativeID] = aggregationOperator.Aggregate(aggregatedCriterias);
            }

            return aggregatedResults.OrderByDescending(tuple =>  tuple.Value.IndexTermino + tuple.Value.SimbolicTraslation);

        }


        public List<double> GetWeightsOfPreferences (List<UnifiedPreference> preferences , string atribute)
        {
            List<double> weigths = new List<double>();
            if (atribute == "expert")
            {
                //Obtener los pesos de los expertos involucrados en este grupo
                weigths = preferences
                .Select(preference => preference.expert.Weight) 
                .ToList();
            }
            if(atribute == "criteria")
            {
                //Obtener los pesos de los criterios involucrados en este grupo
                weigths = preferences
                .Select(preference => preference.criteria.Weight) 
                .ToList();
            }

            // Normalize expert weights to ensure they sum to 1 (optional but recommended)
            double totalWeight = weigths.Sum();
            if (totalWeight > 0) // Avoid division by zero
                weigths = weigths.Select(weight => weight / totalWeight).ToList();

            return weigths;
        }

        public string UnifyRealToLinguisticDomain(float realValue, DomainModel linguisticDomain)
        {
            if (linguisticDomain.Type != DomainType.Linguistico)
                throw new ArgumentException("El dominio proporcionado no es un dominio lingüístico.");

            if (linguisticDomain.FuncionesPertenencia == null || linguisticDomain.TerminosLinguisticos == null)
                throw new InvalidOperationException("El dominio lingüístico no tiene términos o funciones de pertenencia definidos.");

            if (linguisticDomain.FuncionesPertenencia.Count != linguisticDomain.TerminosLinguisticos.Count)
                throw new InvalidOperationException("El número de funciones de pertenencia no coincide con el número de términos lingüísticos.");

            // Variables para rastrear el mejor grado de pertenencia y el término correspondiente
            double maxMembership = double.MinValue;
            string selectedTerm = null;

            // Iterar sobre cada función de pertenencia
            for (int i = 0; i < linguisticDomain.FuncionesPertenencia.Count; i++)
            {
                var membershipFunction = linguisticDomain.FuncionesPertenencia[i];
                double membershipValue = membershipFunction.CalculateMembership(realValue); // Evalúa el grado de pertenencia

                if (membershipValue > maxMembership)
                {
                    maxMembership = membershipValue;
                    selectedTerm = linguisticDomain.TerminosLinguisticos[i];
                }
            }

            // Devuelve el término lingüístico con el mayor grado de pertenencia
            return selectedTerm ?? "Ningún término aplica";
        }

        public Dictionary<string, double> UnifyIntervalToLinguisticDomain(float lowerBound, float upperBound, DomainModel linguisticDomain)
        {
            if (linguisticDomain.Type != DomainType.Linguistico)
                throw new ArgumentException("El dominio proporcionado no es un dominio lingüístico.");

            if (linguisticDomain.FuncionesPertenencia == null || linguisticDomain.TerminosLinguisticos == null)
                throw new InvalidOperationException("El dominio lingüístico no tiene términos o funciones de pertenencia definidos.");

            if (linguisticDomain.FuncionesPertenencia.Count != linguisticDomain.TerminosLinguisticos.Count)
                throw new InvalidOperationException("El número de funciones de pertenencia no coincide con el número de términos lingüísticos.");

            if (lowerBound > upperBound)
                throw new ArgumentException("El límite inferior no puede ser mayor que el límite superior.");

            // Diccionario para almacenar los grados de pertenencia para cada término
            var membershipValues = new Dictionary<string, double>();

            // Resolución para discretizar el intervalo
            int resolution = 100;

            // Iterar sobre cada función de pertenencia
            for (int i = 0; i < linguisticDomain.FuncionesPertenencia.Count; i++)
            {
                var membershipFunction = linguisticDomain.FuncionesPertenencia[i];
                var term = linguisticDomain.TerminosLinguisticos[i];

                // Calcular el máximo grado de pertenencia para el intervalo [lowerBound, upperBound]
                double maxMembership = double.MinValue;

                for (int j = 0; j <= resolution; j++)
                {
                    // Discretizar el intervalo
                    float x = lowerBound + j * (upperBound - lowerBound) / resolution;

                    // Evaluar el grado de pertenencia
                    double membershipValue = membershipFunction.CalculateMembership(x);

                    // Actualizar el máximo grado de pertenencia
                    if (membershipValue > maxMembership)
                    {
                        maxMembership = membershipValue;
                    }
                }

                // Almacenar el valor máximo de pertenencia para el término
                membershipValues[term] = maxMembership;
            }

            return membershipValues;
        }

        public Dictionary<string, double> UnifyLinguisticToLinguisticDomain(
         string linguisticTerm,
         DomainModel sourceDomain,
         DomainModel targetDomain)
        {
            if (sourceDomain.Type != DomainType.Linguistico || targetDomain.Type != DomainType.Linguistico)
                throw new ArgumentException("Ambos dominios deben ser lingüísticos.");

            if (sourceDomain.FuncionesPertenencia == null || targetDomain.FuncionesPertenencia == null)
                throw new InvalidOperationException("Ambos dominios deben tener funciones de pertenencia definidas.");

            if (sourceDomain.TerminosLinguisticos == null || targetDomain.TerminosLinguisticos == null)
                throw new InvalidOperationException("Ambos dominios deben tener términos lingüísticos definidos.");

            if (sourceDomain.FuncionesPertenencia.Count != sourceDomain.TerminosLinguisticos.Count ||
                targetDomain.FuncionesPertenencia.Count != targetDomain.TerminosLinguisticos.Count)
                throw new InvalidOperationException("El número de funciones de pertenencia no coincide con el número de términos lingüísticos en los dominios.");

            // Caso 1: Si los dominios son iguales
            if (sourceDomain.Equals(targetDomain))
            {
                var result = targetDomain.TerminosLinguisticos.ToDictionary(
                    term => term,
                    term => term == linguisticTerm ? 1.0 : 0.0
                );
                return result;
            }

            // Caso 2: Si los dominios son diferentes
            var fuzzySet = new Dictionary<string, double>();
            int resolution = 1000; // Resolución para discretizar las funciones de pertenencia

            // Obtener la función de pertenencia del término en el dominio de origen
            int sourceIndex = sourceDomain.TerminosLinguisticos.IndexOf(linguisticTerm);
            if (sourceIndex == -1)
                throw new ArgumentException("El término lingüístico no existe en el dominio de origen.");

            var sourceMembershipFunction = sourceDomain.FuncionesPertenencia[sourceIndex];

            // Para cada término lingüístico del dominio objetivo
            foreach (var targetMembershipFunction in targetDomain.FuncionesPertenencia)
            {
                double gamma = double.MinValue;

                // Rango de discretización basado en los extremos de ambas funciones
                double start = Math.Min(sourceMembershipFunction.LeftPoint.X, targetMembershipFunction.LeftPoint.X);
                double end = Math.Max(sourceMembershipFunction.RightPoint.X, targetMembershipFunction.RightPoint.X);

                for (int j = 0; j <= resolution; j++)
                {
                    // Discretizar el rango de la función
                    double y = start + j * (end - start) / resolution;

                    // Evaluar min(μ_sj(y), μ_si(y))
                    double sourceValue = sourceMembershipFunction.CalculateMembership(y);
                    double targetValue = targetMembershipFunction.CalculateMembership(y);
                    double minValue = Math.Min(sourceValue, targetValue);

                    // Actualizar el máximo
                    gamma = Math.Max(gamma, minValue);
                }

                // Guardar el resultado en el conjunto difuso
                fuzzySet[targetMembershipFunction.TerminoName] = gamma;
            }

            return fuzzySet;
        }


    }
}
