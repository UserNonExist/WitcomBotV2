using Discord;
using Discord.Interactions;
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
    public static IAudioService _audioService;
    public static ArtworkService _artworkService;
    public static DiscordClientWrapper _discordClientWrapper;
    
    public static InactivityTrackingOptions _Inactivityoptions = new InactivityTrackingOptions
    {
        DisconnectDelay = TimeSpan.FromMinutes(2),
        PollInterval = TimeSpan.FromMinutes(1),
        TrackInactivity = true
    };
    
    public static async Task Init()
    {
        _discordClientWrapper = new DiscordClientWrapper(Bot.Client);
        
        _audioService = new LavalinkCluster(new LavalinkClusterOptions
        {
            Nodes = new[]
            {
                new LavalinkNodeOptions
                {
                    RestUri = Program.Config.LLRESTUri,
                    WebSocketUri = Program.Config.LLWebSocketUri,
                    Password = Program.Config.LLPassword
                }
            }
        }, _discordClientWrapper);
        
        
        Log.Debug(nameof(Init), "Setting up Lavalink...");
        
        bool connected = false;
        
        while (!connected)
        {
            try
            {
                await _audioService.InitializeAsync();
                connected = true;
            }
            catch (Exception e)
            {
                Log.Error(nameof(Init), $"Lavalink failed to connect. {e}");
            }
            await Task.Delay(5000);
        }

        _artworkService = new ArtworkService();
        
        var service = new InactivityTrackingService(_audioService, 
            _discordClientWrapper,
            new InactivityTrackingOptions());
        
        service.BeginTracking();

        Log.Info(nameof(Init), "Lavalink connected.");
    }

    public static async ValueTask<VoteLavalinkPlayer> GetPlayerAsync(bool connectToVoiceChannel = true, ShardedInteractionContext context = null)
    {
        var player = _audioService.GetPlayer<VoteLavalinkPlayer>(context.Guild.Id);

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
        

        var result = await _audioService.JoinAsync<VoteLavalinkPlayer>(user.Guild.Id, user.VoiceChannel.Id);
        
        await result.SetVolumeAsync(0.3f);
        new InactivityTrackingService(_audioService, _discordClientWrapper, _Inactivityoptions);
        
        return result;
    }
}