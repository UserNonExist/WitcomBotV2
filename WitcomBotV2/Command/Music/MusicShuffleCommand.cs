using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("shuffle", "สลับเพลงในคิว", runMode: RunMode.Async)]
    public async Task Shuffle()
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);
        
        if (player == null)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.NoMusicClass));
            return;
        }
        
        if (player.Queue.IsEmpty)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงในคิว", Color.Red), ephemeral: true);
        }
        
        player.Queue.Shuffle();
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "สลับเพลงในคิวแล้ว", Color.Blue));
    }
}