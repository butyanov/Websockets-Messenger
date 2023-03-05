using System.Net;
using System.Net.WebSockets;
using System.Text;
using WebSocketsServer.Bot;

namespace WebSocketsServer;

public class Server
{
    private readonly List<User> _clients = new();
    private readonly HttpListener _listener = new();
    private readonly Logger _logger;

    public Server(string serverName, IEnumerable<string> prefixes)
    {
        foreach (var s in prefixes)
            _listener.Prefixes.Add(s);
        _listener.Start();
        _logger = new Logger(serverName);
        _logger.Log(Action.Started, "Server started");
    }
    
    public async Task Listen()
    {
        _logger.Log(Action.Listening, "Listening for connections...");
        while (_listener.IsListening)
        {
            var context = await _listener.GetContextAsync();
            if (!context.Request.IsWebSocketRequest) continue;
            var connectedUser = await HandleConnection(context);
            try
            {
                _ = Task.Run(async () => await ListenClient(connectedUser));
            }
            catch (WebSocketException)
            {
                _clients.Remove(connectedUser);
                _logger.Log(Action.Disconnected, $"Client '{connectedUser.Name}' disconnected");
            }
        }
    }
    
    private async Task ListenClient(User listeningUser)
    {
        var buffer = new byte[1024];

        while (listeningUser.Socket.State == WebSocketState.Open)
        {
            var result = await listeningUser.Socket.ReceiveAsync(new ArraySegment<byte>(buffer), CancellationToken.None);
            if (result.CloseStatus.HasValue) break;

            var message = Encoding.UTF8.GetString(buffer, 0, result.Count);
            var chatBotResponse = ChatBot.GetResponse(message);

            _logger.Log(Action.Message, $"{message}");
            foreach (var client in _clients.Where(client => client.Socket.State == WebSocketState.Open))
            {
                await client.Socket.SendAsync(new ArraySegment<byte>(buffer, 0, result.Count), WebSocketMessageType.Text, true, CancellationToken.None);
            }

            if (chatBotResponse == null) continue;
            _logger.Log(Action.Message, $"{chatBotResponse}");
            foreach (var client in _clients.Where(client => client.Socket.State == WebSocketState.Open))
            {
                await client.Socket.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(chatBotResponse)), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }

        _logger.Log(Action.Disconnected, $"Client '${listeningUser.Name}' disconnected");
    }
    
    private async Task<User> HandleConnection(HttpListenerContext context)
    {
        var remoteEndpoint = context.Request.RemoteEndPoint.ToString();
        var connection = await context.AcceptWebSocketAsync(null);
        var user = new User(remoteEndpoint, connection.WebSocket);
        
        _clients.Add(user);

        _logger.Log(Action.Connected, $"Client '{user.Name}' connected");
        _logger.Log(Action.ListOfUsers, _clients.Select(c => c.Name).ToList());
        
        return user;
    }
}