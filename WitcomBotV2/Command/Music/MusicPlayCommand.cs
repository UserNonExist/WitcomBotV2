using System.Net;
using System.Net.Security;
using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [DefaultMemberPermissions(GuildPermission.Speak | GuildPermission.Connect | GuildPermission.SendMessages)]
    [SlashCommand("play", "เล่นเพลง", runMode: RunMode.Async)]
    public async Task Play(string query)
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);

        if (player == null)
        {
            return;
        }

        if (query.Contains("list="))
        {
            var response = await MusicModule._audioService.LoadTracksAsync(query, SearchMode.YouTube);
            
            List<LavalinkTrack> playlist = response.Tracks.ToList();
            
            await player.SetVolumeAsync(0.25f);

            for (int i = 0; i < playlist.Count; i++)
            {
                LavalinkTrack playlistTrack = playlist[i];
                await player.PlayAsync(playlistTrack, enqueue: true);
            }

            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music",
                $"เพิ่ม {playlist.Count} เพลงไปยังคิวแล้ว", Color.Blue));
        }

        var track = await MusicModule._audioService.GetTrackAsync(query, SearchMode.YouTube);

        if (track == null)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีผลการค้นหา", Color.Gold));
            return;
        }

        await player.SetVolumeAsync(0.25f);

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