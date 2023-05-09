using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("clear", "ล้างคิวเพลง", runMode: RunMode.Async)]
    public async Task Clear()
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

        player.Queue.Clear();
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ล้างคิวเพลงแล้ว", Color.Blue));
    }
}