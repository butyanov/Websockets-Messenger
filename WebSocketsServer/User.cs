using System.Net.WebSockets;

namespace WebSocketsServer;

public class User
{
    public string Name { get; set; }
    public WebSocket Socket { get; set; }

    public User(string name, WebSocket socket)
    {
        Name = name;
        Socket = socket;
    }
}