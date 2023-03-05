using System.Windows.Input;
using DynamicData;
using ReactiveUI;

namespace WebSocketsUI.ViewModels;

public class MainWindowViewModel : ViewModelBase
{
    private readonly PageViewModelBase[] _pages = new PageViewModelBase[3];
    private PageViewModelBase _currentPage;
    
    public MainWindowViewModel()
    {
        _pages[0] = new FirstPageViewModel();
        _pages[1] = new SecondPageViewModel();
        _pages[2] = new ThirdPageViewModel(_pages[1]);
        
        _currentPage = _pages[0];
            
        var canNavNext = this.WhenAnyValue(x => x.CurrentPage.CanNavigateNext);
        var canNavPrev = this.WhenAnyValue(x => x.CurrentPage.CanNavigatePrevious);

        NavigateNextCommand = ReactiveCommand.Create(NavigateNext, canNavNext);
        NavigatePreviousCommand = ReactiveCommand.Create(NavigatePrevious, canNavPrev);
    }

    public PageViewModelBase CurrentPage
    {
        get => _currentPage;
        private set => this.RaiseAndSetIfChanged(ref _currentPage, value);
    }
        
    public ICommand NavigateNextCommand { get; }

    private void NavigateNext()
    {
        var index = _pages.IndexOf(CurrentPage) + 1;
            
        CurrentPage = _pages[index];
    }
        
    public ICommand NavigatePreviousCommand { get; }

    private void NavigatePrevious()
    {
        var index = _pages.IndexOf(CurrentPage) - 1;
            
        CurrentPage = _pages[index];
    }
}