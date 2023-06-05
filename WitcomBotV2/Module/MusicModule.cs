using Discord;
using Discord.Interactions;
using Discord.Rest;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.Artwork;
using Lavalink4NET.Cluster;
using Lavalink4NET.DiscordNet;
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
        
        AudioService = new LavalinkNode(new LavalinkNodeOptions
                {
                    RestUri = Program.Config.LLRESTUri,
                    WebSocketUri = Program.Config.LLWebSocketUri,
                    Password = Program.Config.LLPassword
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
                Log.Error(nameof(Init), $"Lavalink failed to connect. {e}");
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
    
}