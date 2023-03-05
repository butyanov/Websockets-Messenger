using System;

namespace WebSocketsUI.ViewModels;

public class FirstPageViewModel : PageViewModelBase
{
    public string Title => "Welcome to WebSockets Chat UI";
    
    public string Message => "Press 'Next' to register yourself.";
    
    public override bool CanNavigateNext 
    { 
        get => true;
        protected set => throw new NotSupportedException(); 
    }
    
    public override bool CanNavigatePrevious
    {
        get => false;
        protected set => throw new NotSupportedException();
    }
}