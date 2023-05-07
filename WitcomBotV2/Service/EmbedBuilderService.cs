namespace WitcomBotV2.Service;

using Discord;
using System.Reflection;

public class EmbedBuilderService
{
    public static string FooterText => $"WitcomBotV2 | v{Assembly.GetExecutingAssembly().GetName().Version} | User_NotExist";
    
    public static async Task<Embed> CreateBasicEmbed(string title, string description, Color color)
    {
        Log.Info(nameof(CreateBasicEmbed), $"Sending embed {title}.");

        return await Task.Run(() =>
            new EmbedBuilder().WithTitle(title).WithDescription(description).WithColor(color).WithCurrentTimestamp()
                .WithFooter(FooterText).Build());
    }
}