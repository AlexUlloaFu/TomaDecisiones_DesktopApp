using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Windows.Input;
using Tesis_Project.Core;
using Tesis_Project.Models;
using Tesis_Project.Models.Domains;
using Tesis_Project.Views.Modals;

namespace Tesis_Project.ViewModels
{
    public class DefinirMarcoViewModel : ViewModel
    {
        public ICommand ShowAddExpertCommand { get; }
        public ICommand ShowAddCriteriaCommand { get; }
        public ICommand ShowAddAlternativeCommand { get; }
        public ICommand ShowAddDomainCommand { get; }


        public ObservableCollection<ExpertModel> Experts { get; }
        public ExpertModel _selectedExpert;
        public ExpertModel SelectedExpert
        {
            get => _selectedExpert;
            set
            {
                if (_selectedExpert != value)
                {
                    _selectedExpert = value;
                    OnPropertyChanged(nameof(SelectedExpert));
                }
            }
        }
        public ICommand EditExpertCommand { get; }
        public ICommand RemoveExpertCommand { get; }

        public ObservableCollection<CriteriaModel> Criterias { get; }
        public CriteriaModel _selectedCriteria;
        public CriteriaModel SelectedCriteria
        {
            get => _selectedCriteria;
            set
            {
                if (_selectedCriteria != value)
                {
                    _selectedCriteria = value;
                    OnPropertyChanged(nameof(SelectedCriteria));
                }
            }
        }
        public ICommand EditCriteriaCommand { get; }
        public ICommand RemoveCriteriaCommand { get; }


        public ObservableCollection<AlternativeModel> Alternatives { get; }
        public AlternativeModel _selectedAlternative;
        public AlternativeModel SelectedAlternative
        {
            get => _selectedAlternative;
            set
            {
                if (_selectedAlternative != value)
                {
                    _selectedAlternative = value;
                    OnPropertyChanged(nameof(SelectedAlternative));
                }
            }
        }
        public ICommand EditAlternativeCommand { get; }
        public ICommand RemoveAlternativeCommand { get; }

        public ObservableCollection<DomainModel> Domains { get; }
        public DomainModel _selectedDomain;
        public DomainModel SelectedDomain
        {
            get => _selectedDomain;
            set
            {
                if (_selectedDomain != value)
                {
                    _selectedDomain = value;
                    OnPropertyChanged(nameof(SelectedDomain));
                }
            }
        }
        public ICommand EditDomainCommand { get; }
        public ICommand RemoveDomainCommand { get; }


        public DefinirMarcoViewModel()
        {
            ShowAddExpertCommand = new RelayCommand(param => ShowModal("expertModel"));
            Experts = MarcoDecisionModel.Instance.Experts;
            //EditExpertCommand = new RelayCommand(EditExpert, CanEditOrRemoveExpert);
            RemoveExpertCommand = new RelayCommand((param) => RemoveSelectedItem("expert"), CanEditOrRemoveExpert);

            ShowAddCriteriaCommand = new RelayCommand(param => ShowModal("criteriaModel"));
            Criterias = MarcoDecisionModel.Instance.Criterias;
            //EditCriteriaCommand = new RelayCommand(EditExpert, CanEditOrRemoveCriteria);
            RemoveCriteriaCommand = new RelayCommand((param) => RemoveSelectedItem("criteria"), CanEditOrRemoveCriteria);

            ShowAddAlternativeCommand = new RelayCommand(param => ShowModal("alternativeModel"));
            Alternatives = MarcoDecisionModel.Instance.Alternatives;
            //EditAlternativeCommand = new RelayCommand(EditAlternative, CanEditOrRemoveAlternative);
            RemoveAlternativeCommand = new RelayCommand((param) => RemoveSelectedItem("alternative"), CanEditOrRemoveAlternative);

            ShowAddDomainCommand = new RelayCommand(param => ShowModal("domainModel"));
            Domains = MarcoDecisionModel.Instance.Domains;
            //EditAlternativeCommand = new RelayCommand(EditAlternative, CanEditOrRemoveAlternative);
            RemoveDomainCommand = new RelayCommand((param) => RemoveSelectedItem("domain"), CanEditOrRemoveDomain);
        }

        private void ShowModal(string modelType)
        {
            var modal = new AddItemModal();
            var context = new AddItemModalViewModel(modelType);
            context.CloseAction = () => modal.Close();
            modal.DataContext = context;
            modal.ShowDialog();
        }

        private bool CanEditOrRemoveExpert(object parameter) => SelectedExpert != null;
        private bool CanEditOrRemoveCriteria(object parameter) => SelectedCriteria != null;
        private bool CanEditOrRemoveAlternative(object parameter) => SelectedAlternative != null;
        private bool CanEditOrRemoveDomain(object parameter) => SelectedAlternative != null;

        private void EditExpert(object parameter)
        {
            if (SelectedExpert != null)
            {
                // Edit logic for the selected expert
                SelectedExpert.Name = "New Name"; // Example edit action
                OnPropertyChanged(nameof(Experts));
                Debug.WriteLine("Selected Expert changed: " + _selectedExpert?.Name);
            }
        }

        private void RemoveSelectedItem(string selectedModel)
        {
            if (selectedModel == "expert" && SelectedExpert != null)
            {
                Experts.Remove(SelectedExpert);
                for (int i = 0; i < Experts.Count; i++)
                    Experts[i].Index = i + 1;
            }
            if (selectedModel == "criteria" && SelectedCriteria != null)
            {
                Criterias.Remove(SelectedCriteria);
                for (int i = 0; i < Criterias.Count; i++)
                    Criterias[i].Index = i + 1;
            }
            if (selectedModel == "alternative" && SelectedAlternative != null)
            {
                Alternatives.Remove(SelectedAlternative);
                for (int i = 0; i < Alternatives.Count; i++)
                    Alternatives[i].Index = i + 1;
            }
            if (selectedModel == "alternative" && SelectedAlternative != null)
            {
                Domains.Remove(SelectedDomain);
                for (int i = 0; i < Domains.Count; i++)
                    Domains[i].Index = i + 1;
            }
        }

        public override void OnNavigate()
        {
            
        }

        //public override void OnNavigate()
        //{
        //    throw new NotImplementedException();
        //}
    }
}
