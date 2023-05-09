﻿using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Lavalink4NET.Player;
using Lavalink4NET.Rest;
using Microsoft.Extensions.DependencyInjection;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;


public class MusicModule: InteractionModuleBase<SocketInteractionContext>
{
    public static IAudioService _audioService;
    public static async Task Init()
    {
        _audioService = new LavalinkNode(new LavalinkNodeOptions
        {
            RestUri = "http://localhost:2333/",
            WebSocketUri = "ws://localhost:2333/",
            Password = "youshallnotpass"
        }, new DiscordClientWrapper(Bot.Client));
        
        Log.Debug(nameof(Init), "Setting up Lavalink...");
        
        bool connected = false;
        
        while (!connected)
        {
            try
            {
                await _audioService.InitializeAsync();
                connected = true;
            }
            catch (Exception)
            {
                Log.Error(nameof(Init), "Lavalink failed to connect. Retrying...");
            }
            await Task.Delay(5000);
        }
        Log.Info(nameof(Init), "Lavalink connected.");
    }

    public static async ValueTask<VoteLavalinkPlayer> GetPlayerAsync(bool connectToVoiceChannel = true, SocketInteractionContext context = null)
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

        return await _audioService.JoinAsync<VoteLavalinkPlayer>(user.Guild.Id, user.VoiceChannel.Id);
    }
}