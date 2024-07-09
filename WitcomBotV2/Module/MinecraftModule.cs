using System.ComponentModel;
using mcswlib.ServerStatus;
using mcswlib.ServerStatus.Event;

namespace WitcomBotV2.Module;

public class MinecraftModule
{
    public ServerStatusFactory ServerStatusFactory { get; set; }
    
    public Dictionary<PlayerPayLoad, int> PlayerList { get; set; }
    
    public void Init()
    {
        ServerStatusFactory = new ServerStatusFactory();
        PlayerList = new Dictionary<PlayerPayLoad, int>();
        
        ServerStatusFactory.ServerChanged += OnFactoryChanged;
        
        ServerStatusFactory.Make("localhost", 25565, false, "wc");
        ServerStatusFactory.StartAutoUpdate();
    }
    
    public void OnFactoryChanged(object sender, EventBase[] e)
    {
        Console.WriteLine("Server changed");
        foreach (var evt in e)
        {
            Console.WriteLine(evt);
        }
        
        var entry = ServerStatusFactory.Entries.FirstOrDefault(x => x.Label == "wc");

        foreach (var player in entry.PlayerList)
        {
            if (PlayerList.ContainsKey(player))
            {
                PlayerList[player]++;
            }
            else
            {
                PlayerList.Add(player, 1);
            }
        }
        
        foreach (var player in PlayerList)
        {
            if (!entry.PlayerList.Contains(player.Key))
            {
                PlayerList.Remove(player.Key);
            }
        }
        
    }
}
