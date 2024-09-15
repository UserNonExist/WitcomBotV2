using mcswlib.ServerStatus.Event;

namespace WitcomBotV2.Service;

public class MinecraftServerInfo
{
    public string SrvRecord { get; set; }
    public string Host { get; set; }
    public int Port { get; set; }
    public string Info { get; set; }
    public string Motd { get; set; }
    public string SpecialNotice { get; set; }
    public bool IsOnline { get; set; }
    public List<MinecraftPlayerInfo> Players { get; set; } = new();
}