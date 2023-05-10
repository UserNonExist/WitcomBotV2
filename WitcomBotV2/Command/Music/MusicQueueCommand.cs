using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("queue", "ดูคิวเพลง", runMode: RunMode.Async)]
    public async Task Queue()
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);
        
        if (player == null)
        {
            return;
        }

        if (player.Queue.IsEmpty)
        {
            await RespondAsync(
                embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงในคิว", Color.Purple));
            return;
        }
        
        EmbedBuilder embedBuilder = new();
        embedBuilder.WithColor(Color.Purple);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Queue");
        embedBuilder.WithFooter(EmbedBuilderService.FooterText);
        
        int count = 0;

        foreach (var track in player.Queue.Tracks)
        {
            count += 1;
            embedBuilder.AddField($"{count}. {track.Title}", track.Uri);
            
            if (count % 15 == 0)
            {
                await ReplyAsync(embed: embedBuilder.Build());


                embedBuilder = new();
                embedBuilder.WithColor(Color.Purple);
                embedBuilder.WithCurrentTimestamp();
                embedBuilder.WithTitle("Queue Contd.");
                embedBuilder.WithFooter(EmbedBuilderService.FooterText);
            }
        }

        var components = await Modal.MusicModal.CreateButton();

        await RespondAsync(embed: embedBuilder.Build(), ephemeral: false, components: components);
    }
}