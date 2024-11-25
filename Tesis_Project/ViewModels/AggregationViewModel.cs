using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tesis_Project.Core;
using Tesis_Project.Models;
using Tesis_Project.Models.Aggregation_Operators;
using Tesis_Project.Models.Domains;

namespace Tesis_Project.ViewModels
{
    public class AggregationViewModel : ViewModel
    {
        public ObservableCollection<DomainModel> DominiosLinguisticos { get; set; }

        private DomainModel _selectedCBTL;
        public DomainModel SelectedCBTL
        {
            get => _selectedCBTL;
            set
            {
                _selectedCBTL = value;
                OnPropertyChanged(nameof(SelectedCBTL));
                ComputacionalModel.Instance.CBTL = value;
                UpdateUnify();
            }
        }

  
        public ObservableCollection<UnifiedPreference> UnifiedPreferences { get; set; }

       
        public AggregationViewModel()
        {
           UnifiedPreferences = new ObservableCollection<UnifiedPreference>();
           BuildDefaultCBTL();
        }



        public void BuildDefaultCBTL ()
        {
            DominiosLinguisticos = new ObservableCollection<DomainModel>();
            var dominiosLinguisticosMarco = MarcoDecisionModel.Instance.Domains.Where(d => d.Type == DomainType.Linguistico).ToList().OrderByDescending((d) => d.TerminosLinguisticos.Count);
            foreach (DomainModel domain in dominiosLinguisticosMarco)
            {
                DominiosLinguisticos.Add(domain);
            }
            SelectedCBTL = DominiosLinguisticos.Count > 0 ? DominiosLinguisticos[0] : null;
        }


        public void UpdateUnify()
        {
            UnifiedPreferences.Clear();
            foreach (PreferenceModel preference in MarcoDecisionModel.Instance.Preferences) {
               var unifiedPreference = ComputacionalModel.Instance.UnifyPreference(preference);
               ComputacionalModel.Instance.UnifiedPreferences.Add(unifiedPreference);
               UnifiedPreferences.Add(unifiedPreference);
            } 
        }

       

        public override void OnNavigate()
        {
            BuildDefaultCBTL();
            UpdateUnify();
        }
    }
}
