using System.Text.RegularExpressions;
using Discord;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;

public class SecureChatModule
{
    public static List<SocketGuildChannel> AvaliableGuildChannel = new();
    public static Dictionary<SocketGuildChannel, SocketGuildChannel> MatchedChannel = new();
    
    public static async Task Init ()
    {
        Bot.Client.MessageReceived += HandleMessage;
    
        MatchMakingLoop();
    }
    
    public static void StartMatchmaking()
    {
        Task.Run(MatchMakingLoop);
    }
    
    public static async Task MatchMakingLoop()
    {
        for (;;)
        {
            await Task.Delay(10000);
            
            Log.Debug(nameof(MatchMakingLoop), "Attempting to matchmake..");
            
            AttepmtMatchmaking();
        }
    }
    
    public static void AttepmtMatchmaking()
    {
        if (AvaliableGuildChannel.Count < 2)
        {
            return;
        }
        
        Log.Debug(nameof(AttepmtMatchmaking), "Requirement met, attempting to matchmake..");
        
        Matchmaking();
    }

    public static async Task HandleMessage(SocketMessage message)
    {
        if (message.Author.IsBot)
            return;

        if (MatchedChannel.ContainsKey(message.Channel as SocketGuildChannel))
        {
            string messageContent = message.Content;

            SocketGuildChannel targetChannel = MatchedChannel[message.Channel as SocketGuildChannel];
            
            await targetChannel.Guild.GetTextChannel(targetChannel.Id).SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"{message.Author.Username} - {messageContent}", Color.Green));
        }

        if (MatchedChannel.ContainsValue(message.Channel as SocketGuildChannel))
        {
            string messageContent = message.Content;

            SocketGuildChannel targetChannel = MatchedChannel.FirstOrDefault(x => x.Value == message.Channel as SocketGuildChannel).Key;

            await targetChannel.Guild.GetTextChannel(targetChannel.Id).SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"{message.Author.Username} - {messageContent}", Color.Green));
        }
    }
    
    public static async Task Matchmaking()
    {
        int channelAmount = AvaliableGuildChannel.Count - 1;

        SocketGuildChannel channel1 = AvaliableGuildChannel[new Random().Next(0, channelAmount)];
        AvaliableGuildChannel.Remove(channel1);
        SocketGuildChannel channel2 = AvaliableGuildChannel[new Random().Next(0, channelAmount)];
        AvaliableGuildChannel.Remove(channel2);
        
        
        MatchedChannel.Add(channel1, channel2);

        await channel1.Guild.GetTextChannel(channel1.Id)
            .SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"หาห้องแชทสำเร็จ! เริ่มทำการเชื่อมต่อ",
                Color.Gold));

        await channel2.Guild.GetTextChannel(channel2.Id).SendMessageAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"หาห้องแชทสำเร็จ! เริ่มทำการเชื่อมต่อ",
                Color.Gold));
    }

    public static async Task DestroyLobby(SocketGuildChannel channel)
    {
        SocketGuildChannel channel1 = null;
        SocketGuildChannel channel2 = null;

        if (MatchedChannel.ContainsKey(channel))
        {
            channel1 = channel;
            channel2 = MatchedChannel[channel];
            
            MatchedChannel.Remove(channel);
        }

        if (MatchedChannel.ContainsValue(channel))
        {
            channel1 = channel;
            channel2 = MatchedChannel.FirstOrDefault(x => x.Value == channel).Key;
            
            MatchedChannel.Remove(MatchedChannel.FirstOrDefault(x => x.Value == channel).Key);
        }
        
        await channel1.Guild.GetTextChannel(channel1.Id)
            .SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"ตัดการเชื่อมต่อสำเร็จ", Color.Gold));

        await channel2.Guild.GetTextChannel(channel2.Id).SendMessageAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"ห้องแชทอีกฝั่งได้ทำการตัดการเชื่อมต่อ",
                Color.Gold));
    }
}