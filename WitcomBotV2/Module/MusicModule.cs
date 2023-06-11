using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Artwork;
using Lavalink4NET.Cluster;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Events;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Lavalink4NET.Tracking;
using Microsoft.Extensions.DependencyInjection;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;


public class MusicModule: InteractionModuleBase<ShardedInteractionContext>
{
    public static IAudioService AudioService;
    public static ArtworkService ArtworkService;
    public static DiscordClientWrapper DiscordClientWrapper;
    
    private static readonly InactivityTrackingOptions Inactivityoptions = new InactivityTrackingOptions
    {
        DisconnectDelay = TimeSpan.FromMinutes(2),
        PollInterval = TimeSpan.FromMinutes(1),
        TrackInactivity = true
    };
    
    public static async Task Init()
    {
        DiscordClientWrapper = new DiscordClientWrapper(Bot.Client);
        List<LavalinkNodeOptions> nodes = new List<LavalinkNodeOptions>();
        
        foreach (var nodeList in Program.Config.LavalinkNodeList)
        {
            nodes.Add(new LavalinkNodeOptions
            {
                RestUri = nodeList.RestUri,
                Password = nodeList.Password,
                WebSocketUri = nodeList.WebSocketUri
            });
        }
        
        AudioService = new LavalinkCluster(new LavalinkClusterOptions
        {
            Nodes = nodes,
            LoadBalacingStrategy = LoadBalancingStrategies.LoadStrategy,
            StayOnline = true
        }, DiscordClientWrapper);
        
        
        Log.Debug(nameof(Init), "Setting up Lavalink...");
        
        bool connected = false;
        
        while (!connected)
        {
            try
            {
                await AudioService.InitializeAsync();
                connected = true;
            }
            catch (Exception e)
            {
                Log.Error(nameof(Init), $"Lavalink failed to connect. Retrying...");
            }
            await Task.Delay(5000);
        }

        ArtworkService = new ArtworkService();
        AudioService.TrackException += async (sender, args) =>
        {
            Log.Error(nameof(Init), $"Lavalink exception: {args.ErrorMessage}");
        };
        
        var service = new InactivityTrackingService(AudioService, 
            DiscordClientWrapper,
            new InactivityTrackingOptions());
        
        
        
        service.BeginTracking();

        AudioService.TrackStarted += OnTrackStarted;
        AudioService.TrackException += OnTrackException;

        Log.Info(nameof(Init), "Lavalink connected.");
    }

    public static async ValueTask<VoteLavalinkPlayer> GetPlayerAsync(bool connectToVoiceChannel = true, ShardedInteractionContext context = null)
    {
        var player = AudioService.GetPlayer<VoteLavalinkPlayer>(context.Guild.Id);

        if (player != null && player.State != PlayerState.NotConnected
                           && player.State != PlayerState.Destroyed)
        {
            return player;
        }

        var user = context.Guild.GetUser(context.User.Id);

        if (!user.VoiceState.HasValue)
        {
            await context.Interaction.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "คุณไม่ได้อยู่ในห้องเสียง", Color.Red));
            return null;
        }

        if (!connectToVoiceChannel)
        {
            await context.Interaction.RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Music", "บอทไม่ได้อยู่ในห้องเสียง", Color.Red));
            return null;
        }
        

        var result = await AudioService.JoinAsync<VoteLavalinkPlayer>(user.Guild.Id, user.VoiceChannel.Id);
        
        new InactivityTrackingService(AudioService, DiscordClientWrapper, Inactivityoptions);
        
        return result;
    }


    public static async Task OnTrackStarted(object obj, TrackStartedEventArgs args)
    {
        var player = args.Player as VoteLavalinkPlayer;

        if (player == null)
            return;

        var track = player.CurrentTrack;
        var artwork = ArtworkService.ResolveAsync(track);
        var context = (TrackContext)track.Context!;

        var embed = new EmbedBuilder();
        embed.WithTitle("Music");
        embed.WithCurrentTimestamp();
        embed.WithColor(Color.Green);
        embed.WithDescription($"เริ่มเล่นเพลง \n[{track.Title}]({track.Uri}) - {track.Author}\nRequested by: {context.Requester.Mention}");
        embed.WithImageUrl(artwork.ToString());
        embed.WithFooter(EmbedBuilderService.FooterText);

        var channel = context.Channel;
        var guild = context.Guild;

        await guild.GetTextChannel(channel.Id).SendMessageAsync(embed: embed.Build());
    }

    public static async Task OnTrackException(object obj, TrackExceptionEventArgs args)
    {
        var player = args.Player as VoteLavalinkPlayer;
        
        if (player == null)
            return;

        var track = player.CurrentTrack;
        var context = (TrackContext)track.Context!;
        var channel = context.Channel;
        var guild = context.Guild;
        
        var embed = new EmbedBuilder();
        embed.WithTitle("Music");
        embed.WithCurrentTimestamp();
        embed.WithColor(Color.Red);
        embed.WithDescription($"เกิดข้อผิดพลาดในการเล่นเพลง \n[{track.Title}]({track.Uri}) - {track.Author}");
        embed.WithFooter(EmbedBuilderService.FooterText);
        
        await guild.GetTextChannel(channel.Id).SendMessageAsync(embed: embed.Build());
    }
}