using Discord;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.Music;

public partial class MusicCommand
{
    [SlashCommand("queue", "ดูคิวเพลง", runMode: RunMode.Async)]
    public async Task Queue(int page = 1)
    {
        var player = await MusicModule.GetPlayerAsync(true, Context);
        
        if (player == null)
        {
            return;
        }

        if (player.Queue.IsEmpty)
        {
            await RespondAsync(
                embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีเพลงในคิว", Color.Red), ephemeral: true);
            return;
        }
        
        
        EmbedBuilder embedBuilder = new();
        embedBuilder.WithColor(Color.Purple);
        embedBuilder.WithCurrentTimestamp();
        embedBuilder.WithTitle("Queue");
        embedBuilder.WithFooter(EmbedBuilderService.FooterText);
        
        int count = 0;

        List<EmbedBuilder> embeds = new List<EmbedBuilder>();
        
        double totalpage = 1;

        if (player.Queue.Count % 15 != 0)
        {
            totalpage = Math.Floor((double) player.Queue.Count / 15) + 1;
        }
        else
        {
            totalpage = Math.Floor((double) player.Queue.Count / 15);
        }

        foreach (var track in player.Queue.Tracks)
        {
            count += 1;
            var context = (TrackContext)player.CurrentTrack!.Context!;
            embedBuilder.AddField($"{count}. {track.Title} - Requester: {context.Requester.Username}", track.Uri);
            
            if (count % 15 == 0)
            {
                //await ReplyAsync(embed: embedBuilder.Build(), components: components);
                embedBuilder.WithDescription($"หน้าที่ {page} จาก {totalpage}");
                embeds.Add(embedBuilder);


                embedBuilder = new();
                embedBuilder.WithColor(Color.Purple);
                embedBuilder.WithCurrentTimestamp();
                embedBuilder.WithTitle("Queue Contd.");
                embedBuilder.WithDescription($"หน้าที่ {page} จาก {totalpage}");
                embedBuilder.WithFooter(EmbedBuilderService.FooterText);
            }
        }
        
        embedBuilder.WithDescription($"หน้าที่ {page} จาก {totalpage}");
        embeds.Add(embedBuilder);
        
        if (page > totalpage || page < 1)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "ไม่มีหน้านี้", Color.Purple));
            return;
        }
        
        await RespondAsync(embed: embeds[page - 1].Build());
    }
}