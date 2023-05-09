using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("position", "ดูตำแหน่งเพลง", runMode: RunMode.Async)]
    public async Task Position()
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);
        
        if (player == null)
        {
            return;
        }

        if (player.CurrentTrack == null)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงที่กำลังเล่นอยู่", Color.Blue));
            return;
        }
        
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", $"{player.CurrentTrack.Title} - {player.CurrentTrack.Uri}\n\nPosition {player.Position.Position} / {player.CurrentTrack.Duration}", Color.Blue));
    }
}