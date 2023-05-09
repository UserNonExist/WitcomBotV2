using Discord;
using Discord.Interactions;
using Lavalink4NET.Rest;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("play", "เล่นเพลง", runMode: RunMode.Async)]
    public async Task Play(string query)
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);

        if (player == null)
        {
            return;
        }

        var track = await MusicModule._audioService.GetTrackAsync(query, SearchMode.YouTube);

        if (track == null)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีผลการค้นหา", Color.Gold));
            return;
        }

        var position = await player.PlayAsync(track, enqueue: true);

        if (position == 0)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", $"กำลังเล่น {track.Title} - {track.Source}", Color.Blue));
        }
        else
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", $"เพิ่ม {track.Title} ไปยังคิวที่ {position}", Color.Blue));
        }
    }
}