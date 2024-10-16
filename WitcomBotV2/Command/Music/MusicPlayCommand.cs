﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [DefaultMemberPermissions(GuildPermission.Speak | GuildPermission.Connect | GuildPermission.SendMessages)]
    [SlashCommand("play", "เล่นเพลง", runMode: RunMode.Async)]
    public async Task Play(string query, SearchMode searchMode = SearchMode.YouTube)
    {
        await DeferAsync(ephemeral: true);
        
        var player = await MusicModule.GetPlayerAsync(true, Context);

        if (player == null)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.NoMusicClass));
            return;
        }

        if (query.Contains("list=") && !query.Contains("watch?v="))
        {
            var response = await MusicModule.AudioService.LoadTracksAsync(query, SearchMode.YouTube);
            
            List<LavalinkTrack> playlist = response.Tracks.ToList();
            
            await player.SetVolumeAsync(0.25f);

            for (int i = 0; i < playlist.Count; i++)
            {
                LavalinkTrack playlistTrack = playlist[i];
                
                playlistTrack.Context = new TrackContext
                {
                    Requester = Context.User,
                    Guild = Context.Guild,
                    Channel = Context.Channel as SocketTextChannel
                };
                
                await player.PlayAsync(playlistTrack, enqueue: true);
            }

            await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music",
                $"เพิ่ม {playlist.Count} เพลงไปยังคิวแล้ว", Color.Blue), ephemeral: true);
            return;
        }
        
        LavalinkTrack? track = null;

        for (int i = 0; i < 5; i++)
        {
            track = await MusicModule.AudioService.GetTrackAsync(query, searchMode);
            
            Thread.Sleep(1500);

            if (track != null)
            {
                break;
            }
        }

        if (track == null)
        {
            await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีผลการค้นหา", Color.Gold));
            return;
        }

        await player.SetVolumeAsync(0.25f);
        
        track.Context = new TrackContext
        {
            Requester = Context.User,
            Guild = Context.Guild,
            Channel = Context.Channel as SocketTextChannel
        };

        var position = await player.PlayAsync(track, enqueue: true);

        if (position == 0)
        {
            await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", $"กำลังเล่น {track.Title} - {track.Source}", Color.Blue), ephemeral: true);
        }
        else
        {
            await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", $"เพิ่ม {track.Title} ไปยังคิวที่ {position}", Color.Blue), ephemeral: true);
        }
    }
}