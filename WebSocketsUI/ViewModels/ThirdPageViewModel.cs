using System;
using System.Collections.ObjectModel;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Avalonia.Threading;
using ReactiveUI;

namespace WebSocketsUI.ViewModels;

public class ThirdPageViewModel : PageViewModelBase
{
    private readonly PageViewModelBase _secondPageViewModel;
    public readonly ClientWebSocket ClientWebSocket = new();

    private string? _message;
    public string? Message
    {
        get => _message;
        set => this.RaiseAndSetIfChanged(ref _message, value);
    }

    private ObservableCollection<string> _messages = new();
    public ObservableCollection<string> Messages
    {
        get => _messages;
        set => this.RaiseAndSetIfChanged(ref _messages, value);
    }

    public ThirdPageViewModel(PageViewModelBase secondPageViewModel)
    {
        _secondPageViewModel = secondPageViewModel;
        ClientWebSocket.Options.AddSubProtocol("chat");
        ClientWebSocket.Options.KeepAliveInterval = TimeSpan.FromSeconds(30);
        ClientWebSocket.Options.SetBuffer(1024, 1024);

        Task.Run(async () =>
        {
            await ReceiveMessages();
        });
    }

    private string? BoundUsername => ((SecondPageViewModel)_secondPageViewModel).Username;

    public override bool CanNavigateNext
    {
        get => false;
        protected set => throw new NotSupportedException();
    }
    
    public override bool CanNavigatePrevious
    {
        get => true;
        protected set => throw new NotSupportedException();
    }

    private async Task ReceiveMessages()
    {
        await ClientWebSocket.ConnectAsync(new Uri("ws://localhost:8080/ws"), CancellationToken.None);
        while (ClientWebSocket.State == WebSocketState.Open)
        {
            var buffer = new byte[1024];
            var cReceive = await ClientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, cReceive.Count);
            Dispatcher.UIThread.Post(() =>
            {
                Messages.Add(receivedMessage);
            });
        }
    }

    public void SendMessage()
    {
        var messageWithName = $"{BoundUsername}: {Message}";
        var messageBuffer = Encoding.UTF8.GetBytes(messageWithName);
        ClientWebSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true,
            CancellationToken.None);
        Message = string.Empty;
    }
}