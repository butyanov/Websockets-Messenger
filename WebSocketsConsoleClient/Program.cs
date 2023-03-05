using WebSocketsConsoleClient;

Console.WriteLine("Enter your name:");
var client = new Client(Console.ReadLine() ?? "Anonymous", "ws://localhost:8080");
Task.WaitAll(
    Task.Run(async () => await client.ReceiveMessages()),
        Task.Run(async () => await client.SendMessages())
);