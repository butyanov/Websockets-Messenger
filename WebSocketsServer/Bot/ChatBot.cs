namespace WebSocketsServer.Bot;

public static class ChatBot
{
    private const string BotName = "AI Assistant";

    private static readonly Dictionary<Command, string> CommandsDictionary = new()
    {
        { Command.Hello, "hello" },
        { Command.Bye, "bye" },
        { Command.Help, "help" },
        { Command.Time, "what is the time" },
        { Command.Date, "what is the date" },
        { Command.Predict, "predict me, " }
    };

    private static readonly Dictionary<Command, Delegate> AnswersDictionary = new()
    {
        { Command.Empty, () => "I don't understand you" },
        { Command.Hello, () => "Hello, my name is " + BotName },
        { Command.Bye, () => "Bye, see you later" },
        { Command.Help, () => "I can help you with the following commands: " + string.Join(", ", Enum.GetNames(typeof(Command))) },
        { Command.Time, () => "The current time is " + DateTime.Now.ToString("HH:mm:ss") },
        { Command.Date, () => "Today is " + DateTime.Now.ToString("dd.MM.yyyy") },
        { Command.Predict, Predict },
    };

    public static string? GetResponse(string message)
    {
        var command = GetCommand(message);
        return command == Command.Unrecognized ? null : $"{BotName} : {AnswersDictionary[command].DynamicInvoke()}";
    }
    
    private static Command GetCommand(string message)
    {
        if (!message.Contains("Bot, ")) return Command.Unrecognized;
        message = message.Substring(message.IndexOf("Bot, ", StringComparison.Ordinal) + 5);

        return (from command in CommandsDictionary where message.ToLower().Contains($"{command.Value}") select command.Key).FirstOrDefault();
    }

    private static string Predict()
    {
        string[] options = {"Yes.", "No.", "Maybe.", "I don't know..."};
        
        var random = new Random();
        var index = random.Next(options.Length);
        return options[index];
    }
}