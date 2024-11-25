using System.Windows;
using Tesis_Project.Core;
using Tesis_Project.Models;
using Tesis_Project.Services;
using Tesis_Project.ViewModels;
using Microsoft.Extensions.DependencyInjection;


namespace Tesis_Project
{
    public partial class App : Application
    {
        private ServiceProvider _serviceProvider;

        public App()
        {
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton<MainWindow>(provider => new MainWindow
            {
                DataContext = provider.GetRequiredService<MainViewModel>()
            });
            services.AddSingleton<MainViewModel>();
            services.AddSingleton<DefinirMarcoViewModel>();
            services.AddSingleton<GatheringViewModel>();
            services.AddSingleton<AggregationViewModel>();
            services.AddSingleton<RankingViewModel>();
            services.AddSingleton<INavigationService,NavigationService>();
            services.AddSingleton<Func<Type, ViewModel>>(serviceProvider => viewModelType =>
            {
                return (ViewModel)serviceProvider.GetRequiredService(viewModelType);
            });

            _serviceProvider = services.BuildServiceProvider();
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            var marco = MarcoDecisionModel.Instance;
            //var gatheringViewModel = _serviceProvider.GetRequiredService<GatheringViewModel>();
            
            //// Para que cuando cambien las colecciones se actualicen sus datos en el gathering
            //marco.Experts.CollectionChanged += gatheringViewModel.OnMarcoDecisionCollectionChanged;
            //marco.Criterias.CollectionChanged += gatheringViewModel.OnMarcoDecisionCollectionChanged;
            //marco.Alternatives.CollectionChanged += gatheringViewModel.OnMarcoDecisionCollectionChanged;
            //marco.Domains.CollectionChanged += gatheringViewModel.OnMarcoDecisionCollectionChanged;
            // TO DO: Hacer ese tipo de suscripciones para cada vista

            var mainWindow = _serviceProvider.GetRequiredService<MainWindow>();
            mainWindow.Show();
            base.OnStartup(e);
        }
    }

}
