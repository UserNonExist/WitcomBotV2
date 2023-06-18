using Lavalink4NET;
using Lavalink4NET.Cluster;
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
    public string OpenAIAPIKey { get; set; }
    public string UnloadPassword { get; set; }
    public int TotalShards { get; set; }

    public List<LavalinkServerOption> LavalinkNodeList { get; set; } = new()
    {
        new LavalinkServerOption()
        {
        RestUri = "http://localhost:2333",
        WebSocketUri = "ws://localhost:2333",
        Password = "youshallnotpass",
        }
    };
    
    public ulong SecureChatGuildId { get; set; }
    public ulong SecureChatChannelId { get; set; }
}