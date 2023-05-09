using Discord;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;

public class PingCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("ping", "ดู latency ของบอท")]
    public async Task Ping()
    {
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Ping", $"latency อยู่ที่ {Context.Client.Latency}ms\n\nShard ID: {Context.Client.ShardId}" , Color.Gold),
            ephemeral: false);
    }
}