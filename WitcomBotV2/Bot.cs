﻿using Discord.Interactions;
using Discord.WebSocket;
using Discord.Commands;
using Discord;
using Discord.Rest;
using Lavalink4NET;
using Lavalink4NET.DiscordNet;
using Microsoft.Extensions.DependencyInjection;
using OpenAI_API;
using WitcomBotV2.Command;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2;

public class Bot
{
    private static DiscordShardedClient _client;
    private SocketGuild? _guild;

    public SocketGuild Guild => _guild ??= Client.Guilds.FirstOrDefault(g => g.Id == 979024475729305630);
    public static DiscordShardedClient Client => _client ??= new DiscordShardedClient(new DiscordSocketConfig
        { AlwaysDownloadUsers = true, MessageCacheSize = 10000, TotalShards = 2});

    public InteractionService InteractionService { get; private set; } = null!;
    public SlashCommandHandler SlashCommandHandler { get; private set; } = null!;

    public static Bot Instance { get; private set; } = null!;
    
    public void Destroy() => _client.LogoutAsync();

    public Bot(string[] args)
    {
        Instance = this;
        Init(args).GetAwaiter().GetResult();
    }

    private async Task Init(string[] arg)
    {
        try
        {
            TokenUtils.ValidateToken(TokenType.Bot, Program.Config.BotToken);
        }
        catch (Exception e)
        {
            Log.Error(nameof(Init), e);
            await Task.Delay(-1);
            return;
        }
        
        Log.Debug(nameof(Init), "Bot token validated.");
        
        Log.Debug(nameof(Init), "Initializing Text Commands..");

        Log.Debug(nameof(Init), "Initializing Slash commands..");
        InteractionService = new InteractionService(Client);
        SlashCommandHandler = new SlashCommandHandler(InteractionService, Client);

        Log.Debug(nameof(Init), "Setting up VC renting...");
        Client.UserVoiceStateUpdated += ChannelRenting.OnVoiceStateChanged;

        Log.Debug(nameof(Init), "Setting up logging..");
        InteractionService.Log += Log.Send;
        
        Log.Debug(nameof(Init), "Setting up message handlers..");
        Client.MessageReceived += PingTriggers.HandleMessage;

        Log.Debug(nameof(Init), "Installing Slash commands..");
        await SlashCommandHandler.InstallCommandAsync();


        Client.ShardReady += async async =>
        {
            Log.Debug(nameof(Init), "Initializing Database..");
            await DatabaseHandler.Init(arg.Contains("--updatetables"));
            Log.Debug(nameof(Init), "Initializing OpenAIAPI..");
            await GPTModule.Init();
            Log.Debug(nameof(Init), "Initializing MusicModule..");
            await MusicModule.Init();
            
            Log.Debug(nameof(Init), "Registering Slash commands..");
            int slashCommandsRegistered = (await InteractionService.RegisterCommandsToGuildAsync(Guild.Id)).Count;

            Log.Debug(nameof(Init), $"Registered {slashCommandsRegistered} interaction modules.");
            Log.Debug(nameof(Init), $"All modules initialized. Bot {Client.CurrentUser.Username} ready.");
        };
        
        
        Log.Debug(nameof(Init), "Logging in...");
        await Client.LoginAsync(TokenType.Bot, Program.Config.BotToken);
        await Client.StartAsync();
        _ = Task.Run(() => Client.SetGameAsync("Counter Strike 2", null, ActivityType.Watching));
        Log.Debug(nameof(Init), $"Logged in");

        await Task.Delay(-1);
    }
}