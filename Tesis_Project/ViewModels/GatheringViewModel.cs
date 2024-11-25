using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Diagnostics;
using System.DirectoryServices.ActiveDirectory;
using System.Windows.Input;
using Tesis_Project.Core;
using Tesis_Project.Models;
using Tesis_Project.Models.Domains;
using CommunityToolkit.Mvvm.Input;
using System.Text.RegularExpressions;
using LiveChartsCore.Defaults;

namespace Tesis_Project.ViewModels
{
    public class GatheringViewModel : Core.ViewModel
    {
        public ISeries[] Series { get; set; } 

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

        private int _selectedTermIndex;
        public int SelectedTermIndex
        {
            get => _selectedTermIndex;
            set
            {
                _selectedTermIndex = value;
                OnPropertyChanged(nameof(SelectedTermIndex));
            }
        }




        public PreferenceModel _selectedPreference;
        public PreferenceModel SelectedPreference
        {
            get => _selectedPreference;
            set
            {
                _selectedPreference = value;
                OnPropertyChanged(nameof(SelectedPreference));
                OnPropertyChanged(nameof(IsPreferenceSelected));
                OnPropertyChanged(nameof(IsIntervalarDomainVisible));
                OnPropertyChanged(nameof(IsLinguisticDomainVisible));
                OnPropertyChanged(nameof(IsRealDomainVisible));
                UpdateFuncionesPertenencia();
            }
        }
   
        public bool IsPreferenceSelected => SelectedPreference != null && SelectedPreference.ID != null || SelectedPreference.expert != null;
        public bool IsLinguisticDomainVisible => SelectedPreference != null && SelectedPreference.Domain != null && SelectedPreference.Domain.Type == DomainType.Linguistico;
        public bool IsIntervalarDomainVisible => SelectedPreference != null && SelectedPreference.Domain != null && SelectedPreference.Domain.Type == DomainType.Intervalar;
        public bool IsRealDomainVisible => SelectedPreference != null && SelectedPreference.Domain != null && SelectedPreference.Domain.Type == DomainType.Real;


        public ObservableCollection<DomainModel> DominioValues { get; set; }

        public ObservableCollection<PreferenceModel> Preferences  => MarcoDecisionModel.Instance.Preferences;

        public ICommand EvaluarCommand { get; }
        public ICommand LimpiarEvaluacionCommand { get; }



        public GatheringViewModel()
        {
            EvaluarCommand = new Core.RelayCommand(_ => EvaluarPreferencia());
            LimpiarEvaluacionCommand = new Core.RelayCommand(_ => LimpiarEvaluacionPreferencia());
            Preferences.CollectionChanged += Preferences_CollectionChanged;
        }

        public override void OnNavigate()
        {
            _selectedPreference = new PreferenceModel();
            BuildPreferences();
        }

        public void Preferences_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (PreferenceModel item in e.NewItems)
                {
                    item.DomainChanged += OnDomainChanged;
                }
            }

            if (e.OldItems != null)
            {
                foreach (PreferenceModel item in e.OldItems)
                {
                    // Unsubscribe from DomainChanged event when an item is removed
                    item.DomainChanged -= OnDomainChanged;
                }
            }
        }


        private void UpdateFuncionesPertenencia()
        {
            if (SelectedPreference == null || SelectedPreference.Domain == null) return;
            List<ISeries> list = new List<ISeries>();
            var funcionesPertenencia = SelectedPreference.Domain.FuncionesPertenencia;
            foreach (var triangularFunction in funcionesPertenencia)
            {
                Models.MathUtils.Point leftPoint = triangularFunction.LeftPoint;
                Models.MathUtils.Point peakPoint = triangularFunction.PeakPoint;
                Models.MathUtils.Point rightPoint = triangularFunction.RightPoint;

                // Create a new LineSeries for this triangular function and set up points
                var fuzzySetSeries = new LineSeries<ObservablePoint>
                {
                    Values = [new ObservablePoint(leftPoint.X, leftPoint.Y), new ObservablePoint(peakPoint.X, peakPoint.Y), new ObservablePoint(rightPoint.X, rightPoint.Y)],
                    LineSmoothness = 0,
                    GeometrySize = 1, // Optional: size of the geometry points (e.g., circles at each point)
                    Name = triangularFunction.TerminoName // Label each series with the term name
                };
                list.Add(fuzzySetSeries);
            }
            Series = list.ToArray();
            OnPropertyChanged(nameof(Series));
        }
        public void OnDomainChanged()
        {
            OnPropertyChanged(nameof(IsIntervalarDomainVisible));
            OnPropertyChanged(nameof(IsLinguisticDomainVisible));
            OnPropertyChanged(nameof(IsRealDomainVisible));
            UpdateFuncionesPertenencia();
        }

        private void BuildPreferences()
        {
            var marco = MarcoDecisionModel.Instance;
            var preferences = marco.Preferences;

            foreach (var expert in marco.Experts)
            {
                foreach (var alternative in marco.Alternatives)
                {
                    foreach (var criteria in marco.Criterias)
                    {
                        PreferenceModel newPreference = new PreferenceModel { ID = Guid.NewGuid().ToString(), expert = expert, criteria = criteria, alternative = alternative };
                        if (isPreferenceInMarco(newPreference))
                            continue;
                        
                        marco.Preferences.Add(newPreference);

                    }
                }
            }

            DominioValues = marco.Domains;
      
        }

        private bool isPreferenceInMarco (PreferenceModel newPreference)
        {
            var preferences = MarcoDecisionModel.Instance.Preferences;
            foreach (PreferenceModel preference in preferences)
            {
                if(preference.expert.ID == newPreference.expert.ID && preference.alternative.ID == newPreference.alternative.ID)
                {
                    if(preference.criteria.ID == newPreference.criteria.ID)
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        private void EvaluarPreferencia()
        {
            if (IsRealDomainVisible) {
                _selectedPreference.RealValue = RealValue;
                _selectedPreference.LowerLimit = -1;
                _selectedPreference.UpperLimit = -1;
                _selectedPreference.SelectedTerminoLinguistico = "";
            }
            if (IsIntervalarDomainVisible) {
                _selectedPreference.LowerLimit = LowerLimit;
                _selectedPreference.UpperLimit = UpperLimit;
                _selectedPreference.RealValue = -1;
                _selectedPreference.SelectedTerminoLinguistico = "";
            }
            if (IsLinguisticDomainVisible) {
                _selectedPreference.SelectedTerminoLinguistico = SelectedTerminoLinguistico;
                _selectedPreference.IndexTermino = SelectedTermIndex;
                _selectedPreference.LowerLimit = -1;
                _selectedPreference.UpperLimit = -1;
                _selectedPreference.RealValue = -1;
            }
            OnPropertyChanged(nameof(Preferences));
            OnPropertyChanged(nameof(SelectedPreference));
            OnPropertyChanged(nameof(MarcoDecisionModel.Instance.Preferences));
        }

        private void LimpiarEvaluacionPreferencia()
        {

        }

        public void ResetSelections()
        {
            SelectedPreference = new PreferenceModel();
        }
    }
}
