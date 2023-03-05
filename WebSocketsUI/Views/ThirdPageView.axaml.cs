using Avalonia.Controls;
using Avalonia.Interactivity;
using WebSocketsUI.ViewModels;

namespace WebSocketsUI.Views;

public partial class ThirdPageView : UserControl
{
    public ThirdPageView()
    {
        InitializeComponent();
    }
    
    public void OnSendMessage(object? sender, RoutedEventArgs args)
    {
        var context = DataContext as ThirdPageViewModel;
        
        context?.SendMessage();
    }
}