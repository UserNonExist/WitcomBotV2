using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("skip", "ข้ามเพลง", runMode: RunMode.Async)]
    public async Task Skip()
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

        if (player.Queue.IsEmpty)
        {
            await player.StopAsync();
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงในคิว หยุดการเล่นเพลงทั้งหมดแล้ว", Color.Blue));
        }
        
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", $"ข้ามเพลง {player.CurrentTrack.Title}", Color.Blue));
        await player.SkipAsync();
    }
}