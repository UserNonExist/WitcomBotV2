using System.Security.Cryptography;

namespace WitcomBotV2;

using Newtonsoft.Json;
using System.Reflection;
using Microsoft.Extensions.Hosting;

public class Program
{
    private static Config? _config;
    private static string KCfile = "config.json";
    private static Bot? _bot;
    
    public static string DatabaseFile { get; } = Path.Combine(Environment.CurrentDirectory, "Witcom.db");
    public static Config Config => _config ??= GetConfig();
    public static Random Rng { get; } = new();

    public static void Main(string[] args)
    {
        Console.WriteLine($"WitcomBotV2 v{Assembly.GetExecutingAssembly().GetName().Version}| Initializing...");
        if (args.Contains("--debug"))
            Config.Debug = true;
        _bot = new Bot(args);
        AppDomain.CurrentDomain.ProcessExit += (s, e) => _bot.Destroy();
    }
    
    private static Config GetConfig()
    {
        
        if (!File.Exists(KCfile))
        {
            Console.WriteLine("Config file not found. Creating one...");
            var config = new Config();
            File.WriteAllText(KCfile, JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings()
            {
                ReferenceLoopHandling = ReferenceLoopHandling.Ignore
            }));
            Console.WriteLine("Config file created. Please fill in the token and restart the bot.");
            Console.ReadLine();
            Environment.Exit(0);
        }

        return JsonConvert.DeserializeObject<Config>(File.ReadAllText(KCfile));
    }
}