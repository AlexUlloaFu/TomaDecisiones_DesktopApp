using Microsoft.Win32;
using Newtonsoft.Json;
using System;
using System.IO;
using System.Windows;
using System.Xml;
using Tesis_Project.Core;
using Tesis_Project.Models;
using Tesis_Project.Services;

namespace Tesis_Project.ViewModels
{
    public class MainViewModel : Core.ViewModel
    {

        public RelayCommand NuevoProyectoCommand { get; set; }
        public RelayCommand GuardarProyectoCommand { get; set; }
        public RelayCommand CargarProyectoCommand { get; set; }
        public RelayCommand CerrarAplicacionCommand { get; set; }


        private INavigationService _navigationService;
        public INavigationService NavigationService
        {
            get => _navigationService;
            set
            {
                _navigationService = value;
                OnPropertyChanged();
            }
        }

        public RelayCommand NavigateDefinirMarcoCommand { get; set; }
        public RelayCommand NavigateGatheringCommand { get; set; }
        public RelayCommand NavigateAggregationCommand { get; set; }
        public RelayCommand NavigateRankingCommand { get; set; }


        private bool _canNavigateToAggregation;
        public bool CanNavigateToAggregation
        {
            get { return _canNavigateToAggregation; }
            set
            {
                _canNavigateToAggregation = value;
                OnPropertyChanged(nameof(CanNavigateToAggregation));
            }

        }
        private bool _canNavigateToRanking;
        public bool CanNavigateToRanking
        {
            get { return _canNavigateToRanking; }
            set
            {
                _canNavigateToRanking = value;
                OnPropertyChanged(nameof(CanNavigateToRanking));
            }

        }

        public MainViewModel(INavigationService navService)
        {

            // Initialize commands
            NuevoProyectoCommand = new RelayCommand(_ => CreateNewProject());
            GuardarProyectoCommand = new RelayCommand(_ => SaveProject());
            CargarProyectoCommand = new RelayCommand(_ => LoadProject());
            CerrarAplicacionCommand = new RelayCommand(_ => Application.Current.Shutdown());

            NavigateDefinirMarcoCommand = new RelayCommand(_ => NavigationService.Navigate<DefinirMarcoViewModel>());
            NavigateGatheringCommand = new RelayCommand(_ => OnGatheringPressed());
            NavigateAggregationCommand = new RelayCommand(_ => OnAggregationPressed());
            NavigateRankingCommand = new RelayCommand(_ => NavigationService.Navigate<RankingViewModel>());
            
            // Set default navigation
            NavigationService = navService;
            NavigationService.Navigate<DefinirMarcoViewModel>();

        }

        private void OnGatheringPressed()
        {
            CanNavigateToAggregation = true;
            NavigationService.Navigate<GatheringViewModel>();
        }
        private void OnAggregationPressed()
        {
            CanNavigateToRanking = true;   
            NavigationService.Navigate<AggregationViewModel>();
        }
     

        private void CreateNewProject()
        {
            // Clear all collections in the instance to start a new project
            MarcoDecisionModel.Instance.Experts.Clear();
            MarcoDecisionModel.Instance.Criterias.Clear();
            MarcoDecisionModel.Instance.Alternatives.Clear();
            MarcoDecisionModel.Instance.Domains.Clear();
            MarcoDecisionModel.Instance.Preferences.Clear();

            MessageBox.Show("Nuevo proyecto creado!", "Información", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void SaveProject()
        {
            try
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog
                {
                    Title = "Guardar Proyecto",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments),
                    FileName = "Proyecto.json"
                };

                if (saveFileDialog.ShowDialog() == true)
                {
                    string filePath = saveFileDialog.FileName;
                    string jsonData = JsonConvert.SerializeObject(MarcoDecisionModel.Instance, Newtonsoft.Json.Formatting.Indented);
                    File.WriteAllText(filePath, jsonData);
                    MessageBox.Show("Proyecto guardado con éxito!", "Guardar Proyecto", MessageBoxButton.OK, MessageBoxImage.Information);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al guardar proyecto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void LoadProject()
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog
                {
                    Title = "Cargar Proyecto",
                    Filter = "JSON Files (*.json)|*.json|All Files (*.*)|*.*",
                    InitialDirectory = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)
                };

                if (openFileDialog.ShowDialog() == true)
                {
                    string filePath = openFileDialog.FileName;
                    string jsonData = File.ReadAllText(filePath);
                    var loadedData = JsonConvert.DeserializeObject<MarcoDecisionModel>(jsonData);

                    if (loadedData != null)
                    {
                        // Clear current collections and load from the saved data
                        MarcoDecisionModel.Instance.Experts.Clear();
                        foreach (var expert in loadedData.Experts)
                            MarcoDecisionModel.Instance.Experts.Add(expert);

                        MarcoDecisionModel.Instance.Criterias.Clear();
                        foreach (var criteria in loadedData.Criterias)
                            MarcoDecisionModel.Instance.Criterias.Add(criteria);

                        MarcoDecisionModel.Instance.Alternatives.Clear();
                        foreach (var alternative in loadedData.Alternatives)
                            MarcoDecisionModel.Instance.Alternatives.Add(alternative);

                        MarcoDecisionModel.Instance.Domains.Clear();
                        foreach (var domain in loadedData.Domains)
                            MarcoDecisionModel.Instance.Domains.Add(domain);

                        MarcoDecisionModel.Instance.Preferences.Clear();
                        foreach (var preference in loadedData.Preferences)
                            MarcoDecisionModel.Instance.Preferences.Add(preference);

                        MessageBox.Show("Proyecto cargado correctamente!", "Cargar Proyecto", MessageBoxButton.OK, MessageBoxImage.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar proyecto: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        public override void OnNavigate()
        {
            throw new NotImplementedException();
        }
    }
}
