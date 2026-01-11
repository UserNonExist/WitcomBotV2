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
    public int BombModuleHitChance { get; set; }

    public List<MinecraftServerOption> MinecraftServers { get; set; } = new();

    public List<ulong> BombModuleGuildId { get; set; } = new();
}