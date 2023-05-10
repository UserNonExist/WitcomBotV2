using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("remove", "ลบเพลงในคิว", runMode: RunMode.Async)]
    public async Task Remove([Summary("TrackID", "เลขไอดีคิวของเพลง")] int trackid)
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);
        
        if (player == null)
        {
            return;
        }
        
        if (player.Queue.IsEmpty)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงในคิว", Color.Red));
        }
        
        if (trackid > player.Queue.Count || trackid < 1)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีไอดีเพลงนี้ในคิว", Color.Red));
        }
        
        var track = player.Queue.Tracks[trackid - 1];
        player.Queue.Remove(track);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", $"ลบเพลง {track.Title} ออกจากคิวแล้ว", Color.Blue));
    }
}