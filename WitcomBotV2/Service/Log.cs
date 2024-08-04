namespace WitcomBotV2.Service;

using Discord;

public class Log
{
    public static Task Send(LogMessage msg)
    {
        switch (msg.Severity)
        {
            case LogSeverity.Info:
                Console.ForegroundColor = ConsoleColor.White;
                break;
            case LogSeverity.Debug:
                Console.ForegroundColor = ConsoleColor.Blue;
                break;
            case LogSeverity.Warning:
                Console.ForegroundColor = ConsoleColor.Yellow;
                break;
            case LogSeverity.Error:
                Console.ForegroundColor = ConsoleColor.Red;
                break;
        }
        
        Console.WriteLine(msg.ToString());
        Console.ResetColor();
        
        return Task.CompletedTask;
    }

    public static void Info(string source, object msg) => 
        Send(new LogMessage(LogSeverity.Info, source, msg.ToString()));

    public static void Debug(string source, object msg)
    {
        if (Program.Config.Debug)
            Send(new LogMessage(LogSeverity.Debug, source, msg.ToString()));
    }
        
    public static void Error(string source, object msg) => 
        Send(new LogMessage(LogSeverity.Error, source, msg.ToString()));

    public static void Warn(string source, object msg) =>
        Send(new LogMessage(LogSeverity.Warning, source, msg.ToString()));
}