using Discord;
using Discord.Interactions;
using Lavalink4NET.Player;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("loop", "เปิด/ปิดการเล่นซ้ำ")]
    public async Task Loop([Summary("Loop", "เปิด/ปิดการเล่นซ้ำ")] LoopType loopType)
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);
        
        if (player == null)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.NoMusicClass));
            return;
        }
        
        if (player.CurrentTrack == null)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงที่กำลังเล่นอยู่", Color.Red), ephemeral: true);
            return;
        }

        switch (loopType)
        {
            case LoopType.None:
                player.LoopMode = PlayerLoopMode.None;
                await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ปิดการเล่นซ้ำ", Color.Blue));
                break;
            case LoopType.Queue:
                player.LoopMode = PlayerLoopMode.Queue;
                await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "เปิดการเล่นซ้ำทั้งคิว", Color.Blue));
                break;
            case LoopType.Song:
                player.LoopMode = PlayerLoopMode.Track;
                await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "เปิดการเล่นซ้ำเพลง", Color.Blue));
                break;
        }
    }
    
    
    public enum LoopType
    {
        None,
        Queue,
        Song
    }
}