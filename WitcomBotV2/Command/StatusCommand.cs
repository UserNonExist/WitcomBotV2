using Discord;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;

public class StatusCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.SendMessages)]
    [SlashCommand("status", "ดูสถานะของบอท")]
    public async Task Status()
    {
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Status",
            $"กำลังใช้ Shards ที่ {Bot.Client.GetShardIdFor(Context.Guild)+1} จาก {Bot.Client.Shards.Count}\nจำนวนเซิร์ฟเวอร์ปัจจุบัน: {Bot.Client.Guilds.Count}\n\nปิง: {Bot.Client.Latency} ms", Color.Green));
    }
}