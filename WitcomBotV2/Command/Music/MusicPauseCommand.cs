using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("pause", "หยุดเล่นเพลง", runMode: RunMode.Async)]
    public async Task Stop()
    {
        var player = await MusicModule.GetPlayerAsync(connectToVoiceChannel: false, Context);
        
        if (player == null)
        {
            return;
        }

        if (player.CurrentTrack == null)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงที่กำลังเล่นอยู่", Color.Red));
            return;
        }

        if (player.State == PlayerState.Paused)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "เพลงหยุดเล่นอยู่แล้ว", Color.Blue));
        }

        await player.PauseAsync();
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "หยุดเล่นเพลง", Color.Blue));
    }
}