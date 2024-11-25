using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Input;
using Tesis_Project.Core;
using Tesis_Project.Models;
using Tesis_Project.Models.Domains;
using Tesis_Project.Models.MathUtils;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;
using System.Collections;
using Newtonsoft.Json.Linq;
using System.Windows.Media.Media3D;

namespace Tesis_Project.ViewModels
{
    public class AddItemModalViewModel : INotifyPropertyChanged , INotifyDataErrorInfo
    {
        private string _modelType;
        public string ModelType
        {
            get { return _modelType; }
            set
            {
                _modelType = value;
                OnPropertyChanged(nameof(ModelType));
            }
        }
        public string ModalTitle { get; }

        private string _labelName;
        public string LabelName {
            get {  return _labelName; }
            set
            {
                _labelName = value;
                OnPropertyChanged(nameof(LabelName));
                ValidateFieldIsEmpty(nameof(LabelName), value);
            }
        }

        private string _weightText;
        public string WeightText
        {
            get { return _weightText; }
            set
            {
                _weightText = value;
                OnPropertyChanged(nameof(WeightText));
                ValidateFieldIsEmpty(nameof(WeightText), value);
                ValidateFieldIsNumber(nameof(WeightText), value);
            }
        }

        public ObservableCollection<string> aviableDomains { get; } = new ObservableCollection<string> { "Dominio Númerico", "Dominio Intervalar", "Dominio Lingüístico" };
       
        public string _selectedDomain;
        public string SelectedDomain
        {
            get { return _selectedDomain; }
            set
            {
                _selectedDomain = value;
                OnPropertyChanged(nameof(SelectedDomain));
            }
        }
       
        private string _terminoLinguistico;
        public string TextField_TerminoLinguistico
        {
            get { return _terminoLinguistico; }
            set
            {
                _terminoLinguistico = value;
                OnPropertyChanged(nameof(TextField_TerminoLinguistico));
            }
        }
        public ObservableCollection<string> TerminosLinguisticos { get; private set; }
        public ICommand AddTerminoLinguisticoCommand { get; }
        public Collection<TriangularFunction> FuncionesPertenencia { get; private set; }
        public ISeries[] Series { get; set; }


        public ICommand SaveCommand { get; }
        public ICommand CancelCommand { get; }
        public Action CloseAction { get; set; }


        public bool IsAddDomain => ModelType == "domainModel";
        public bool IsNotAddDomain => !IsAddDomain;
        

        private readonly Dictionary<string, List<string>> _errors = new();
        public bool HasErrors => _errors.Count > 0;

        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        public AddItemModalViewModel(string modelType)
        {
            ModelType = modelType;
            SaveCommand = new RelayCommand(param => AddItem());
            CancelCommand = new RelayCommand(param => Cancel());
            AddTerminoLinguisticoCommand = new RelayCommand(param => AddNewTerminoLinguistico());
            TerminosLinguisticos = new ObservableCollection<string>();
            FuncionesPertenencia = new Collection<TriangularFunction>();

            ModalTitle = modelType switch
            {
                "expertModel" => "Adicionar Experto",
                "criteriaModel" => "Adicionar Criterio",
                "alternativeModel" => "Adicionar Alternativa",
                "domainModel" => "Adicionar Dominio de Expresión",
                _ => "Adicionar Elemento"
            };

            SelectedDomain = aviableDomains[0];
        }

        public void AddItem()
        {
            ValidateFieldIsEmpty(nameof(LabelName), LabelName);
            if(ModelType != "domainModel") { 
                ValidateFieldIsEmpty(nameof(WeightText), WeightText);
                ValidateFieldIsNumber(nameof(WeightText), WeightText);
            }
         
            if (HasErrors) return;

            var marcoDecision = MarcoDecisionModel.Instance;

            switch (ModelType)
            {
                case "expertModel":
                   
                    marcoDecision.Experts.Add(new ExpertModel
                    {
                        ID = Guid.NewGuid().ToString(),
                        Name = LabelName,
                        Index = marcoDecision.Experts.Count + 1,
                        Weight = float.Parse(WeightText),

                    });
                    break;

                case "criteriaModel":
                    marcoDecision.Criterias.Add(new CriteriaModel
                    {
                        ID = Guid.NewGuid().ToString(),
                        Name = LabelName,
                        Index = marcoDecision.Criterias.Count + 1,
                        Weight = float.Parse(WeightText),
                    });
                    break;

                case "alternativeModel":
                    marcoDecision.Alternatives.Add(new AlternativeModel
                    {
                        ID = Guid.NewGuid().ToString(),
                        Name = LabelName,
                        Index = marcoDecision.Alternatives.Count + 1,
                        Weight = float.Parse(WeightText),
                    });
                    break;

                case "domainModel":
                    DomainModel domain = new DomainModel
                    {
                        ID = Guid.NewGuid().ToString(),
                        Name = LabelName,
                        Index = marcoDecision.Domains.Count + 1
                    };

                    if (SelectedDomain == "Dominio Númerico")
                        domain.Type = DomainType.Real;
                    else if (SelectedDomain == "Dominio Intervalar")
                    {
                        domain.Type = DomainType.Intervalar;
                    }
                    else if (SelectedDomain == "Dominio Lingüístico")
                    {
                        domain.Type = DomainType.Linguistico;
                        domain.TerminosLinguisticos = TerminosLinguisticos;
                        domain.FuncionesPertenencia = FuncionesPertenencia;
                    }

                    marcoDecision.Domains.Add(domain);
                    break;
            }

            CloseModal();
        }

        private void AddNewTerminoLinguistico()
        {
            ValidateFieldIsEmpty(nameof(TextField_TerminoLinguistico), TextField_TerminoLinguistico);
            if (HasErrors) return;

            TerminosLinguisticos.Add(TextField_TerminoLinguistico);

            List<ISeries> list = new List<ISeries>();
            ComputacionalModel comp_model = ComputacionalModel.Instance;
            FuncionesPertenencia = comp_model.CreateTriangularFunctions(TerminosLinguisticos.ToList(), FuncionesPertenencia);
            foreach (var triangularFunction in FuncionesPertenencia)
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

            TextField_TerminoLinguistico = "";
        }

        private void Cancel() => CloseModal();

        private void CloseModal()
        {
            //ComputacionalModel.Instance.CBTL = new ObservableCollection<TriangularFunction>();
            CloseAction?.Invoke();
        }


        private void ValidateFieldIsEmpty(string propertyName, string value)
        {
            if (_errors.ContainsKey(propertyName))
                _errors.Remove(propertyName);

            if (string.IsNullOrWhiteSpace(value))
                AddError(propertyName, "El campo no puede estar vacío.");

            OnErrorsChanged(propertyName);
        }

        private void ValidateFieldIsNumber(string propertyName, string value)
        {
            if (_errors.ContainsKey(propertyName))
                _errors.Remove(propertyName);
            bool isFloat = float.TryParse(value, out _);
            Trace.WriteLine(isFloat);
            if (!isFloat)
                AddError(propertyName, "El campo tiene que ser un valor numérico");

            OnErrorsChanged(propertyName);
        }

        private void AddError(string propertyName, string error)
        {
            if (!_errors.ContainsKey(propertyName))
                _errors[propertyName] = new List<string>();

            _errors[propertyName].Add(error);
        }
        public IEnumerable GetErrors(string propertyName)
        {
            if (_errors.ContainsKey(propertyName))
                return _errors[propertyName];
            return null;
        }
        private void OnErrorsChanged(string propertyName) =>
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

    }
}
