using System.Net.WebSockets;
using System.Text;

namespace WebSocketsConsoleClient;

public class Client
{
    private readonly string _userName; 
    private readonly ClientWebSocket _clientWebSocket = new();
    private readonly string _connectionStr;
    public Client(string userName, string connectionStr)
    {
        _userName = userName;
        _connectionStr = connectionStr;
    }

    public async Task ReceiveMessages()
    {
        await _clientWebSocket.ConnectAsync(new Uri(_connectionStr), CancellationToken.None);
        Console.WriteLine("Connected to server. Type your message and press Enter to send. Type 'exit' to close the connection.");
        while (_clientWebSocket.State == WebSocketState.Open)
        {
            var buffer = new byte[1024];
            var cReceive = await _clientWebSocket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            var receivedMessage = Encoding.UTF8.GetString(buffer, 0, cReceive.Count);
            Console.WriteLine(receivedMessage);
        }
    }

    public async Task SendMessages()
    {
        while (true)
        {
            var message = Console.ReadLine();
            if (message?.ToLower() == "exit")
            {
                await _clientWebSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, "", CancellationToken.None);
                break;
            }
                
            var messageWithName = $"{_userName}: {message}";
            var messageBuffer = Encoding.UTF8.GetBytes(messageWithName);
            await _clientWebSocket.SendAsync(new ArraySegment<byte>(messageBuffer), WebSocketMessageType.Text, true, CancellationToken.None);
        }
    }
}