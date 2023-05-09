using Discord;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;

public class StatusCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("status", "ดูสถานะของบอท")]
    public async Task Status()
    {
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Status",
            $"จำนวน Shards: {Bot.Client.Shards.Count}\n\nPing: {Bot.Client.Latency}", Color.Gold));
    }
}