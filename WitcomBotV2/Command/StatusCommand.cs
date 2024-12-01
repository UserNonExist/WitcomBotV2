using Discord;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;

public class StatusCommand : InteractionModuleBase<SocketInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.SendMessages)]
    [SlashCommand("status", "ดูสถานะของบอท")]
    public async Task Status()
    {
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Status",
            $"ปิง: {Bot.Client.Latency} ms", Color.Green));
    }
}