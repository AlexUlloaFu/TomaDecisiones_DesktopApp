using System.ComponentModel;
using System.Diagnostics;
using System.Runtime.CompilerServices;
using Tesis_Project.Models.Domains;
using Tesis_Project.Models.MathUtils;

namespace Tesis_Project.Models
{
    public class PreferenceModel : INotifyPropertyChanged
    {

        private string _ID;
        public string ID
        {
            get => _ID;
            set
            {
                _ID = value;
            }
        }

        private ExpertModel _expert;
        public ExpertModel expert
        {
            get => _expert;
            set
            {
                _expert = value;
            }
        }

        private CriteriaModel _criteria;
        public CriteriaModel criteria
        {
            get => _criteria;
            set
            {
                _criteria = value;
            }
        }

        private AlternativeModel _alternative;
        public AlternativeModel alternative
        {
            get => _alternative;
            set
            {
                _alternative = value;
            }
        }

        private DomainModel _domain;
        public DomainModel Domain
        {
            get => _domain;
            set
            {
                if (value == null) return;
                _domain = value;
                OnPropertyChanged(nameof(IsLinguisticPreference));
                OnPropertyChanged(nameof(IsIntervalarPreference));
                OnPropertyChanged(nameof(IsRealPreference));
                OnPropertyChanged(nameof(Domain));
                DomainChanged?.Invoke();
            }
        }

        public Linguistic2Tuple Tuple;

        public string FormattedTuple => Tuple != null ? '(' + Domain.TerminosLinguisticos[Tuple.IndexTermino] + ',' + Tuple.SimbolicTraslation + ')' : "(0,0)";

        private float _realValue;
        public float RealValue
        {
            get => _realValue;
            set
            {
                if (_realValue != value)
                {
                    _realValue = value;
                    OnPropertyChanged(nameof(RealValue));
                }
            }
        }


        private string _selectedTerminoLingustico;
        public string SelectedTerminoLinguistico
        {
            get => _selectedTerminoLingustico;
            set
            {
                _selectedTerminoLingustico = value;
                OnPropertyChanged(nameof(SelectedTerminoLinguistico));
            }
        }

        private int _indexTermino;
        public int IndexTermino
        {
            get => _indexTermino;
            set
            {
                _indexTermino = value;
            }
        }


        private string _intervalarValue;
        public string IntervalarValue
        {
            get => _intervalarValue;
            set
            {
                _intervalarValue = value;
                OnPropertyChanged(nameof(IntervalarValue));
            }
        }

        private float _lowerLimit;
        public float LowerLimit
        {
            get => _lowerLimit;
            set
            {
                if (_lowerLimit != value)
                {
                    _lowerLimit = value;
                    OnPropertyChanged(nameof(LowerLimit));
                    IntervalarValue = "[" + value + "," + UpperLimit + "]";
                }
            }
        }

        private float _upperLimit;
        public float UpperLimit
        {
            get => _upperLimit;
            set
            {
                if (_upperLimit != value)
                {
                    _upperLimit = value;
                    OnPropertyChanged(nameof(UpperLimit));
                    IntervalarValue = "[" + LowerLimit + "," + value + "]";
                }
            }
        }

        // Event to signal a change in the entire Domain object
        public event Action DomainChanged;

        public bool IsLinguisticPreference => Domain != null && Domain.Type == DomainType.Linguistico;
        public bool IsIntervalarPreference => Domain != null && Domain.Type == DomainType.Intervalar;
        public bool IsRealPreference => Domain != null && Domain.Type == DomainType.Real;


        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }
}
