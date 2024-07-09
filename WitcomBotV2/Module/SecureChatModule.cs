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
    
    public static async Task MatchMakingLoop()
    {
        for (;;)
        {
            await Task.Delay(10000);
            
            AttemptMatchmaking();
        }
    }
    
    public static void AttemptMatchmaking()
    {
        if (AvaliableGuildChannel.Count < 2)
        {
            return;
        }
        
        Log.Debug(nameof(AttemptMatchmaking), "Requirement met, attempting to matchmake..");
        
        Matchmaking();
    }

    public static async Task HandleMessage(SocketMessage message)
    {
        if (message.Author.IsBot)
            return;

        if (MatchedChannel.ContainsKey(message.Channel as SocketGuildChannel))
        {
            string messageContent = message.Content;
            
            messageContent += message.Attachments.Count > 0 ? $"\n`{message.Attachments.First().Url}`" : "";

            SocketGuildChannel targetChannel = MatchedChannel[message.Channel as SocketGuildChannel];
            
            await targetChannel.Guild.GetTextChannel(targetChannel.Id).SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed($"{message.Author.GlobalName} - Secure Chat", $"{messageContent}", Color.Purple, imgthumb: message.Author.GetDisplayAvatarUrl()));
        }

        if (MatchedChannel.ContainsValue(message.Channel as SocketGuildChannel))
        {
            string messageContent = message.Content;
            
            messageContent += message.Attachments.Count > 0 ? $"\n`{message.Attachments.First().Url}`" : "";

            SocketGuildChannel targetChannel = MatchedChannel.FirstOrDefault(x => x.Value == message.Channel as SocketGuildChannel).Key;

            await targetChannel.Guild.GetTextChannel(targetChannel.Id).SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed($"{message.Author.GlobalName} - Secure Chat", $"{messageContent}", Color.Purple, imgthumb: message.Author.GetDisplayAvatarUrl()));
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
            .SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"หาห้องแชทสำเร็จ!",
                Color.Green));

        await channel2.Guild.GetTextChannel(channel2.Id).SendMessageAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"หาห้องแชทสำเร็จ!",
                Color.Green));
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
            .SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"ตัดการเชื่อมต่อสำเร็จ", Color.Red));

        await channel2.Guild.GetTextChannel(channel2.Id).SendMessageAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"อีกห้องแชทได้ทำการตัดการเชื่อมต่อ",
                Color.Red));
    }

    public static async Task DisconnectAllLobby()
    {
        foreach (var channel in MatchedChannel)
        {
            await channel.Key.Guild.GetTextChannel(channel.Key.Id)
                .SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", "บอทถูกตัดการเชื่อมต่อ", Color.Orange));
            await channel.Value.Guild.GetTextChannel(channel.Value.Id)
                .SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", "บอทถูกตัดการเชื่อมต่อ", Color.Orange));
            
            MatchedChannel.Remove(channel.Key);
        }
    }
}