using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.Models;
using Tesis_Project.Models.Aggregation_Operators;
using Tesis_Project.Models.MathUtils;

namespace Tesis_Project.ViewModels
{
    public class RankingViewModel : Core.ViewModel
    {

        public ObservableCollection<IAggregationOperator> Operators { get; set; }

        private IAggregationOperator _selectedOperator;
        public IAggregationOperator SelectedOperator
        {
            get => _selectedOperator;
            set
            {
                _selectedOperator = value;
                OnPropertyChanged(nameof(SelectedOperator));
                ComputacionalModel.Instance.aggregationOperator = value;
                BuildRanking();
            }
        }

        public ObservableCollection<RankingItem> Ranking { get; private set; }



        public RankingViewModel()
        {
        }

        public override void OnNavigate()
        {
            DefineOperators();
            BuildRanking();
        }

        public void DefineOperators()
        {
            Operators = new ObservableCollection<IAggregationOperator>();
            IAggregationOperator media = new MediaAritmetica2Tupla();
            Operators.Add(media);
            SelectedOperator = Operators[0];
        }

        public void BuildRanking()
        {
            //ComputacionalModel.Instance.CBTL = MarcoDecisionModel.Instance.Domains[0].FuncionesPertenencia;
            Ranking = new ObservableCollection<RankingItem>();
            MediaAritmetica2Tupla mediaAritmetica2Tupla = new MediaAritmetica2Tupla();
            var alternatives = MarcoDecisionModel.Instance.Alternatives;
            var ranking = ComputacionalModel.Instance.AggregatePreferences();
            foreach (var alternativeAggResult in ranking)
            {
                string alternativeID = alternativeAggResult.Key;
                Linguistic2Tuple tuple = alternativeAggResult.Value;
                RankingItem item = new RankingItem();
                foreach (AlternativeModel alternative in alternatives)
                {
                    if (alternative.ID == alternativeID) item.Alternative = alternative.Name;
                }
                item.Value = '(' + ComputacionalModel.Instance.CBTL.TerminosLinguisticos[tuple.IndexTermino] + ',' + Double.Round(tuple.SimbolicTraslation,2).ToString() + ')';
                Ranking.Add(item);
            }
        }

    }

    public struct RankingItem
    {
        public string Alternative { get; set; }
        public string Value { get; set; }

        public RankingItem(string alternative, string value)
        {
            Alternative = alternative;
            Value = value;
        }
    }
}
