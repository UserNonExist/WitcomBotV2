using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Minecraft;

public partial class MinecraftCommand
{
    [SlashCommand("status", "แสดงสถานะของเซิร์ฟเวอร์")]
    public async Task GetStatus()
    {
        await DeferAsync();

        var lists = Bot.Instance.MinecraftModule.ServerStatusFactory.Entries;

        var embed = new EmbedBuilder()
            .WithTitle("Minecraft")
            .WithDescription($"# สถานะของเซิร์ฟเวอร์\n<updateTxt>\n\n\n")
            .WithColor(Color.Blue)
            .WithFooter(EmbedBuilderService.FooterText)
            .WithCurrentTimestamp();

        string updateTime = "";
        
        foreach (var entry in lists)
        {
            embed.Description += $"## `{entry.Label}` - ";
            embed.Description += entry.IsOnline ? ":green_square: ออนไลน์" : ":red_square: ออฟไลน์";
            embed.Description += $"\n";
            
            if (entry.IsOnline)
            {
                embed.Description += $"**MOTD:** {entry.MOTD}\n";
                embed.Description += $"**Players:** {entry.PlayerCount}/{entry.MaxPlayerCount}\n\n";
            }
            else
            {
                embed.Description += "\n";
            }

            if (updateTime == "")
            {
                updateTime = entry.LastStatusDate;
            }
        }
        
        embed.Description += "ใช้คำสั่ง `/minecraft info <เซิร์ฟเวอร์แอดเดรส | server address>` เพื่อดูข้อมูลเพิ่มเติม";
        embed.Description = embed.Description.Replace("<updateTxt>", $"### อัพเดทล่าสุด: {updateTime}");
        
        await FollowupAsync(embed: embed.Build());
    }
}