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
            string avaliableServers = "";
            foreach (var srv in Bot.Instance.MinecraftModule.MinecraftServerInfos)
            {
                avaliableServers += "`" + srv.SrvRecord + "`\n";
                avaliableServers += srv.Motd + "\n\n";
            }
            
            await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed($"Minecraft - {serverAddress}", "ไม่พบข้อมูลของเซิร์ฟเวอร์นี้\nเซิร์ฟเวอร์แอดเดรสที่ใส่ได้\n\n\n " + avaliableServers, Color.Red), ephemeral: true);
            return;
        }
        
        if (!entry.IsOnline)
        {
            await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed($"Minecraft - {serverAddress}", "เซิร์ฟเวอร์นี้ออฟไลน์\n\nถ้าหากว่าเกิดปัญหาขึ้นให้แจ้งด้วย", Color.Red));
            return;
        }
        
        var mcInfo = Bot.Instance.MinecraftModule.MinecraftServerInfos.FirstOrDefault(x => x.SrvRecord == serverAddress);

        var embed = new EmbedBuilder()
            .WithTitle($"Minecraft - {serverAddress} | {entry.PlayerCount}/{entry.MaxPlayerCount} คนออนไลน์")
            .WithDescription($"# ข้อมูลของเซิร์ฟเวอร์\nMOTD: {mcInfo.Motd}\n\nข้อมูลเพิ่มเติม: {mcInfo.Info}\n\n")
            .WithColor(Color.Blue)
            .WithFooter(EmbedBuilderService.FooterText)
            .WithCurrentTimestamp();
        
        if (mcInfo.Players.Count == 0)
        {
            embed.Description += "ไม่มีผู้เล่นออนไลน์";
            embed.AddField("ผู้เล่น", "-", true);
            embed.AddField("เวลาออนไลน์", "-", true);
            await FollowupAsync(embed: embed.Build());
            return;
        }
        
        string name = "";
        string time = "";

        foreach (var ply in mcInfo.Players)
        {
            var duration = (DateTime.Now - ply.FirstConnect).Duration();
            name += ply.Name + "\n";
            time += $"{duration.Hours} ชั่วโมง {duration.Minutes} นาที" + "\n";
        }
        
        embed.AddField("ผู้เล่น", name, true);
        embed.AddField("เวลาออนไลน์", time, true);
        
        await FollowupAsync(embed: embed.Build());
    }
}