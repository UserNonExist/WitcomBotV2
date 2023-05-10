using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("goto", "ไปยังเพลงที่ต้องการในคิว", runMode: RunMode.Async)]
    public async Task Goto([Summary("TrackID", "เลขไอดีคิวของเพลง")] int trackid)
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
        
        await player.PlayAsync(track, enqueue: false);
        
        for (var trackIndex = 0; trackIndex < trackid; trackIndex++)
        {
            player.Queue.Remove(player.Queue.Tracks[0]);
        }
        
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", $"ข้ามไปยังเพลง {track.Title} แล้ว", Color.Blue));
    }
}