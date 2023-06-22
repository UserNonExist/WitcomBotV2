using Discord;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;

public class SecureChatModule
{
    public static List<SocketGuildChannel> AvaliableGuildChannel = new();
    public static Dictionary<SocketGuildChannel, SocketGuildChannel> MatchedChannel = new();
    
    public static async Task Init (DiscordSocketClient client)
    {
        client.MessageReceived += HandleMessage;
    }

    public static async Task HandleMessage(SocketMessage message)
    {
        if (message.Author.IsBot)
            return;

        if (MatchedChannel.ContainsKey(message.Channel as SocketGuildChannel))
        {
            string messageContent = message.Content;

            SocketGuildChannel targetChannel = MatchedChannel[message.Channel as SocketGuildChannel];
            
            await targetChannel.Guild.GetTextChannel(targetChannel.Id).SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"{message.Author.Username} - {messageContent}", Color.Gold));
        }

        if (MatchedChannel.ContainsValue(message.Channel as SocketGuildChannel))
        {
            string messageContent = message.Content;

            SocketGuildChannel targetChannel = MatchedChannel.FirstOrDefault(x => x.Value == message.Channel as SocketGuildChannel).Key;

            await targetChannel.Guild.GetTextChannel(targetChannel.Id).SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"{message.Author.Username} - {messageContent}", Color.Gold));
        }
    }
    
    public static async Task Matchmaking()
    {
        if (AvaliableGuildChannel.Count < 2)
            return;
        
        int channelAmount = AvaliableGuildChannel.Count - 1;

        int randomChannel1 = 0;
        int randomChannel2 = 0;

        while (randomChannel1 == randomChannel2)
        {
            randomChannel1 = new Random().Next(0, channelAmount);
            randomChannel2 = new Random().Next(0, channelAmount);
        }
        
        SocketGuildChannel channel1 = AvaliableGuildChannel[randomChannel1];
        SocketGuildChannel channel2 = AvaliableGuildChannel[randomChannel2];
        
        MatchedChannel.Add(channel1, channel2);
        
        AvaliableGuildChannel.Remove(channel1);
        AvaliableGuildChannel.Remove(channel2);

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
            embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"ห้องแชทอีกฝั่งได้ตัดการเชื่อมต่อ",
                Color.Gold));
    }
}