using System;
using System.ComponentModel.DataAnnotations;
using ReactiveUI;

namespace WebSocketsUI.ViewModels;

public class SecondPageViewModel : PageViewModelBase
{
    public SecondPageViewModel()
    {
        this.WhenAnyValue(x => x.Username)
            .Subscribe(_ => UpdateCanNavigateNext());
    }

    private string? _username;
    
    [Required]
    [MinLength(3)]
    [MaxLength(25)]
    public string? Username
    {
        get => _username;
        set => this.RaiseAndSetIfChanged(ref _username, value);
    }

    private bool _canNavigateNext;
    
    public override bool CanNavigateNext
    {
        get => _canNavigateNext;
        protected set => this.RaiseAndSetIfChanged(ref _canNavigateNext, value);
    }

    
    public override bool CanNavigatePrevious
    {
        get => true;
        protected set => throw new NotSupportedException();
    }
    
    private void UpdateCanNavigateNext()
    {
        CanNavigateNext =
            !string.IsNullOrEmpty(_username);
    }
}