using Lavalink4NET;
using Lavalink4NET.Rest;
using WitcomBotV2.Service;

namespace WitcomBotV2;

public class Config
{

    public string BotPrefix { get; set; }
    public string BotToken { get; set; }
    public ulong GuildId { get; set; }
    public bool Debug { get; set; }
    public ulong DiscAdminId { get; set; }
    public int TriggerLengthLimit { get; set; }
    public ulong ChannelRentId { get; set; }
    public ulong ChannelRentCatId { get; set; }

    public LavalinkServerOption LavalinkList { get; set; } = new()
    {
        RestUri = "http://localhost:2333",
        WebSocketUri = "ws://localhost:2333",
        Password = "youshallnotpass"
    };
    
    public string MinecraftModpackInfo { get; set; }
}