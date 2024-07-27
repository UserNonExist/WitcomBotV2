using System.ComponentModel;
using mcswlib.ServerStatus;
using mcswlib.ServerStatus.Event;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;

public class MinecraftModule
{
    public ServerStatusFactory ServerStatusFactory { get; set; }
    
    public List<MinecraftServerInfo> MinecraftServerInfos { get; set; } = new();
    
    public void Init()
    {
        ServerStatusFactory = new ServerStatusFactory();
        
        ServerStatusFactory.ServerChanged += OnServerChanged;

        foreach (var mcSvr in Program.Config.MinecraftServers)
        {
            ServerStatusFactory.Make(mcSvr.Host, mcSvr.Port, false, mcSvr.SrvRecord);
            
            MinecraftServerInfos.Add(new MinecraftServerInfo
            {
                SrvRecord = mcSvr.SrvRecord,
                Host = mcSvr.Host,
                Port = mcSvr.Port,
                Info = mcSvr.Info,
                Players = new List<MinecraftPlayerInfo>()
            });
            
            Log.Debug($"{nameof(MinecraftModule)}.{nameof(Init)}", $"Added server {mcSvr.Host}:{mcSvr.Port}");
        }
        
        ServerStatusFactory.StartAutoUpdate();
    }
    
    public void OnServerChanged(object sender, EventBase[] e)
    {
        var server = (ServerStatus)sender;
        
        Log.Debug($"{nameof(MinecraftModule)}.{nameof(OnServerChanged)}", $"Server {server.Label} status updated");

        var mcSvr = MinecraftServerInfos.FirstOrDefault(x => x.SrvRecord == server.Label);
        
        if (mcSvr == null)
        {
            Log.Error($"{nameof(MinecraftModule)}.{nameof(OnServerChanged)}", $"Server {server.Label} not found in MinecraftServerInfos");
            return;
        }
        
        mcSvr.Motd = server.MOTD;

        var players = server.PlayerList;
        
        Dictionary<MinecraftPlayerInfo, bool> checkedPlayers = new();
        
        foreach (var player in mcSvr.Players)
        {
            checkedPlayers.Add(player, false);
        }

        foreach (var playerPayLoad in players)
        {
            var player = mcSvr.Players.FirstOrDefault(x => x.UUID == playerPayLoad.Id);
            
            if (player == null)
            {
                mcSvr.Players.Add(new MinecraftPlayerInfo
                {
                    Name = playerPayLoad.Name,
                    UUID = playerPayLoad.Id,
                    FirstConnect = DateTime.Now
                });
                checkedPlayers.Add(mcSvr.Players.Last(), true);
                
                continue;
            }
            
            if (playerPayLoad.Id == player.UUID)
            {
                checkedPlayers[player] = true;
                continue;
            }
        }

        foreach (var player in checkedPlayers)
        {
            if (!player.Value)
            {
                mcSvr.Players.Remove(player.Key);
            }
        }
    }
}
