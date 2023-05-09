using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

[Group("music", "คำสั่งเกี่ยวกับเพลง")]
public partial class MusicCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("disconnect", "ออกจากห้องเสียง", runMode: RunMode.Async)]
    public async Task Disconnect()
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);
        
        if (player == null)
        {
            return;
        }

        await player.StopAsync(true);
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ออกจากห้องเสียงแล้ว", Color.Blue));
    }
}