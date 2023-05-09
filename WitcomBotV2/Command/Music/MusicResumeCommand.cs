﻿using Discord.Interactions;
using Discord;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("resume", "เล่นเพลงต่อ", runMode: RunMode.Async)]
    public async Task Resume()
    {
        var player = await MusicModule.GetPlayerAsync(connectToVoiceChannel: false, Context);
        
        if (player == null)
        {
            return;
        }

        if (player.CurrentTrack == null)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงที่กำลังเล่นอยู่", Color.Blue));
            return;
        }

        if (player.State == PlayerState.Playing)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "เพลงกำลังเล่นอยู่แล้ว", Color.Blue));
        }

        await player.ResumeAsync();
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "เล่นเพลงต่อ", Color.Blue));
    }
}