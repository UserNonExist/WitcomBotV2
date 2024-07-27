using Discord;
using Discord.Interactions;
using SQLitePCL;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Minecraft;

[Group("minecraft", "คำสั่งที่เกี่ยวกับตัวเซิร์ฟเวอร์ minecraft")]
public partial class MinecraftCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("info", "แสดงข้อมูลเกี่ยวกับเซิร์ฟเวอร์")]
    public async Task GetInfo(string serverAddress)
    {
        if (serverAddress == "")
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Minecraft", "กรุณาใส่เซิร์ฟเวอร์แอดเดรสของเซิร์ฟเวอร์", Color.Red), ephemeral: true);
            return;
        }

        await DeferAsync();
        
        var entry = Bot.Instance.MinecraftModule.ServerStatusFactory.Entries.FirstOrDefault(x => x.Label == serverAddress);
        
        if (entry == null)
        {
            await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed($"Minecraft - {serverAddress}", "ไม่พบข้อมูลของเซิร์ฟเวอร์นี้", Color.Red), ephemeral: true);
            return;
        }
        
        if (!entry.IsOnline)
        {
            await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed($"Minecraft - {serverAddress}", "เซิร์ฟเวอร์นี้ออฟไลน์", Color.Red));
            return;
        }
        
        var mcInfo = Bot.Instance.MinecraftModule.MinecraftServerInfos.FirstOrDefault(x => x.SrvRecord == serverAddress);

        var embed = new EmbedBuilder()
            .WithTitle($"Minecraft - {serverAddress}")
            .WithDescription($"# ข้อมูลของเซิร์ฟเวอร์\nMOTD: {mcInfo.Motd}\n\nข้อมูลเพิ่มเติม: {mcInfo.Info}\n\n")
            .WithColor(Color.Blue)
            .WithFooter(EmbedBuilderService.FooterText);

        int i = 1;

        foreach (var ply in mcInfo.Players)
        {
            var duration = (DateTime.Now - ply.FirstConnect).Duration();
            
            embed.AddField("ลำดับ", i, true);
            embed.AddField("ผู้เล่น", ply.Name, true);
            embed.AddField("เวลาออนไลน์", duration.ToString(@"hh\:mm"), true);
            i += 1;
        }
        
        await FollowupAsync(embed: embed.Build());
    }
}