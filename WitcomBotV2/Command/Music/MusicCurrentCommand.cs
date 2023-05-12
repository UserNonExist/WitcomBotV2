using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("current", "ดูข้อมูลเพลงที่เล่น", runMode: RunMode.Async)]
    public async Task Position()
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);
        
        if (player == null)
        {
            return;
        }

        if (player.CurrentTrack == null)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงที่กำลังเล่นอยู่", Color.Red), ephemeral: true);
            return;
        }
        
        var artwork = await MusicModule._artworkService.ResolveAsync(player.CurrentTrack);
        var context = (TrackContext)player.CurrentTrack.Context!;

        var embed = new EmbedBuilder();
        embed.WithTitle("Music");
        embed.WithCurrentTimestamp();
        embed.WithColor(Color.Green);
        embed.WithDescription($"กำลังเล่นเพลง \n[{player.CurrentTrack.Title}]({player.CurrentTrack.Uri}) - {player.CurrentTrack.Author}\nRequested by: {context.Requester.Mention}\n\n{player.CurrentTrack.Duration}");
        embed.WithImageUrl(artwork.ToString());
        embed.WithFooter(EmbedBuilderService.FooterText);
        
        await RespondAsync(embed: embed.Build());
    }
}