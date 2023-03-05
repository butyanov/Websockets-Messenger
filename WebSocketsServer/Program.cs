using WebSocketsServer;

var server = new Server("itisKFU", new []
{
    "http://localhost:8080/"
});

await server.Listen();