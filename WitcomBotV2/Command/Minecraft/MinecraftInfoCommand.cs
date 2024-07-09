using Discord;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Minecraft;

public partial class MinecraftCommand
{
    [SlashCommand("info", "แสดงข้อมูลเกี่ยวกับเซิร์ฟเวอร์")]
    public async Task GetInfo()
    {
        string info = Program.Config.MinecraftModpackInfo;
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Minecraft", info, Color.DarkGreen), ephemeral: false);
    }
}