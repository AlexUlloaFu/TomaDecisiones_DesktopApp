using Tesis_Project.Core;
using Tesis_Project.ViewModels;

namespace Tesis_Project.Services;

public interface INavigationService
{
    ViewModel CurrentView { get; }
    void Navigate<T>() where T : ViewModel;
}

public class NavigationService : ObservableObject, INavigationService
{
    private readonly Func<Type, ViewModel> _viewModelFactory;
    private ViewModel _currentView;
    public ViewModel CurrentView { 
        get => _currentView; 
        private set {
            _currentView = value;
            OnPropertyChanged();
        } 
    }

    public NavigationService(Func <Type, ViewModel> viewModelFactory)
    {
        _viewModelFactory = viewModelFactory;
    }

    public void Navigate<TViewModel>() where TViewModel : ViewModel
    {
        ViewModel viewModel = _viewModelFactory.Invoke(typeof(TViewModel));
        viewModel.OnNavigate();
        CurrentView = viewModel;
    }

}
