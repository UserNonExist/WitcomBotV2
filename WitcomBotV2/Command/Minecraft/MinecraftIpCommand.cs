using Discord;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Minecraft;

[Group("minecraft", "คำสั่งที่เกี่ยวกับตัวเซิร์ฟเวอร์ minecraft")]
public partial class MinecraftCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("ip", "แสดง IP ของเซิร์ฟเวอร์")]
    public async Task GetIp()
    {
        HttpClient wc = new();
        string ip = await wc.GetStringAsync("https://api.ipify.org");
        
        if (string.IsNullOrEmpty(ip))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.CouldNotGetIp));
            wc.Dispose();
            return;
        }
        
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Minecraft", $"ไอพีของเซิรืฟเวอร์ Minecraft คือ `{ip}`", Color.Green), ephemeral: false);
        
        wc.Dispose();
    }
}