using System.ComponentModel;
using mcswlib.ServerStatus;
using mcswlib.ServerStatus.Event;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;

public class MinecraftModule
{
    public ServerStatusFactory ServerStatusFactory { get; set; }
    
    public List<MinecraftServerInfo> MinecraftServerInfos { get; set; } = new();
    public Dictionary<MinecraftPlayerInfo, CancellationTokenSource> DisconnectingPlayers { get; set; } = new();
    
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
                IsOnline = false,
                SpecialNotice = "",
                Players = new List<MinecraftPlayerInfo>()
            });
            
            Log.Info($"{nameof(MinecraftModule)}.{nameof(Init)}", $"Added server {mcSvr.Host}:{mcSvr.Port}");
        }
        
        ServerStatusFactory.StartAutoUpdate();
    }
    
    public void OnServerChanged(object sender, EventBase[] e)
    {
        var server = (ServerStatus)sender;
        
        Log.Info($"{nameof(MinecraftModule)}.{nameof(OnServerChanged)}", $"Server {server.Label} status updated: ");
        
        var mcSvr = MinecraftServerInfos.FirstOrDefault(x => x.SrvRecord == server.Label);
        
        if (mcSvr == null)
        {
            Log.Error($"{nameof(MinecraftModule)}.{nameof(OnServerChanged)}", $"Server {server.Label} not found in MinecraftServerInfos");
            return;
        }

        if (mcSvr.IsOnline != server.IsOnline)
        {
            mcSvr.IsOnline = server.IsOnline;
            Log.Info($"{nameof(MinecraftModule)}.{nameof(OnServerChanged)}", $"The server is now " + (mcSvr.IsOnline ? "online" : "offline"));
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
            
            if (player != null)
            {
                if (playerPayLoad.Id == player.UUID)
                {
                    checkedPlayers[player] = true;
                    continue;
                }
            }
            
            var dcPlayer = DisconnectingPlayers.FirstOrDefault(x => x.Key.UUID == playerPayLoad.Id);
                
            if (dcPlayer.Key != null)
            {
                mcSvr.Players.Add(new MinecraftPlayerInfo
                {
                    Name = playerPayLoad.Name,
                    UUID = playerPayLoad.Id,
                    FirstConnect = dcPlayer.Key.FirstConnect
                });
                    
                DisconnectingPlayers.FirstOrDefault(x => x.Key.UUID == playerPayLoad.Id).Value.Cancel();
                Log.Info($"{nameof(MinecraftModule)}.{nameof(OnServerChanged)}", $"Player {playerPayLoad.Name} reconnected to the server");
            }
            else
            {
                mcSvr.Players.Add(new MinecraftPlayerInfo
                {
                    Name = playerPayLoad.Name,
                    UUID = playerPayLoad.Id,
                    FirstConnect = DateTime.Now 
                });
                    
                Log.Info($"{nameof(MinecraftModule)}.{nameof(OnServerChanged)}", $"Player {playerPayLoad.Name} joined the server");
            }
                
            checkedPlayers.Add(mcSvr.Players.Last(), true);
        }

        foreach (var player in checkedPlayers)
        {
            if (!player.Value)
            {
                DisconnectingPlayers.Add(player.Key, new CancellationTokenSource());
                DcTimer(player.Key, DisconnectingPlayers[player.Key], 300);
                mcSvr.Players.Remove(player.Key);
                Log.Info($"{nameof(MinecraftModule)}.{nameof(OnServerChanged)}", $"Player {player.Key.Name} left the server, " +
                    $"they have 5 minutes of grace period before playtime is reset");
            }
        }
    }
    
    public async Task DcTimer(MinecraftPlayerInfo dcingPlayer, CancellationTokenSource cancellation, int time)
    {
        while (time > 0)
        {
            if (cancellation.Token.IsCancellationRequested)
            {
                Log.Info($"{nameof(MinecraftModule)}.{nameof(Timer)}", $"Timer for {dcingPlayer.Name} has been cancelled");
                cancellation.Dispose();
                DisconnectingPlayers.Remove(dcingPlayer);
                return;
            }
            
            await Task.Delay(1000);
            time--;
        }
        
        Log.Info($"{nameof(MinecraftModule)}.{nameof(Timer)}", $"Timer for {dcingPlayer.Name} has ended, removing player from disconnecting list");
        
        cancellation.Dispose();
        DisconnectingPlayers.Remove(dcingPlayer);
    }

    public async Task<bool> RemoveAllPlaytime(MinecraftServerInfo mcInfo)
    {
        if (mcInfo.Players.Count == 0)
        {
            Log.Error($"{nameof(MinecraftModule)}.{nameof(RemoveAllPlaytime)}", "No players to remove playtime from");
            return false;
        }
        
        foreach (var player in mcInfo.Players)
        {
            DisconnectingPlayers.FirstOrDefault(x => x.Key.UUID == player.UUID).Value.Cancel();
            DisconnectingPlayers.Remove(player);
        }
        
        mcInfo.Players.Clear();
        
        Log.Info($"{nameof(MinecraftModule)}.{nameof(RemoveAllPlaytime)}", "All playtime has been removed");
        return true;
    }
    
    
}
