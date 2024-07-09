using Discord;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Minecraft;

public partial class MinecraftCommand
{
    [SlashCommand("status", "แสดงสถานะของเซิร์ฟเวอร์")]
    public async Task GetStatus()
    {
        await DeferAsync();
        
        var entry = Bot.Instance.MinecraftModule.ServerStatusFactory.Entries.FirstOrDefault(x => x.Label == "wc");
        
        if (entry == null)
        {
            await FollowupAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, "Cannot find server status entry"));
            return;
        }
        
        string desc = "จำนวนผู้เล่นที่กำลังเล่นอยู่: **" + entry.PlayerCount + "/" + entry.MaxPlayerCount + "**\n\n";
        desc += "ผู้เล่นที่กำลังเล่นอยู่: \n";
        foreach (var player in Bot.Instance.MinecraftModule.PlayerList)
        {
            desc += " + " + player.Key.Name + $" - {player.Value / 2} minute(s) in\n";
        }
        desc += "\n";
        desc += "*เวลาอัปเดต: " + entry.LastStatusDate + "*";
        
        string header = $"Minecraft - " + (entry.IsOnline ? "ออนไลน์" : "ออฟไลน์");
        Color color = entry.IsOnline ? Color.Green : Color.Red;

        await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed(header, desc, color));
    }
}