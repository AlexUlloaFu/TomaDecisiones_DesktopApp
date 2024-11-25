using System.Collections.ObjectModel;
using System.ComponentModel;
using Tesis_Project.Models.Domains;

namespace Tesis_Project.Models
{
    public class MarcoDecisionModel : INotifyPropertyChanged
    {
        private static MarcoDecisionModel _instance;
        public static MarcoDecisionModel Instance
        {
            get
            {
                if (_instance == null) _instance = new MarcoDecisionModel();
                return _instance;
            }
        }

        private MarcoDecisionModel()
        {
            Experts = new ObservableCollection<ExpertModel>();
            Criterias = new ObservableCollection<CriteriaModel>();
            Alternatives = new ObservableCollection<AlternativeModel>();
            Domains = new ObservableCollection<DomainModel>();
            Preferences = new ObservableCollection<PreferenceModel>();
        }

        // Data collections
        public ObservableCollection<ExpertModel> Experts { get; }
        public ObservableCollection<CriteriaModel> Criterias { get; }
        public ObservableCollection<AlternativeModel> Alternatives { get; }
        public ObservableCollection<DomainModel> Domains { get; }
        public ObservableCollection<PreferenceModel> Preferences { get; }


        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
