using Discord;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;

public class SecureChatModule
{
    public static List<SocketGuildChannel> AvaliableGuildChannel = new();
    public static Dictionary<SocketGuildChannel, SocketGuildChannel> MatchedChannel = new();

    public static async Task HandleMessage(SocketMessage message)
    {
        if (message.Author.IsBot)
            return;

        if (MatchedChannel.ContainsKey(message.Channel as SocketGuildChannel))
        {
            string messageContent = message.Content;
            //Log.Debug(nameof(SecureChatModule), $"Message from {message.Author.Username} ({message.Author.Id}): {messageContent}");
            
            SocketGuildChannel targetChannel = MatchedChannel[message.Channel as SocketGuildChannel];
            
            await targetChannel.Guild.GetTextChannel(targetChannel.Id).SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"{message.Author.Username} - {messageContent}", Color.Gold));
        }

        if (MatchedChannel.ContainsValue(message.Channel as SocketGuildChannel))
        {
            string messageContent = message.Content;
            //Log.Debug(nameof(SecureChatModule), $"Message from {message.Author.Username} ({message.Author.Id}): {messageContent}");
            
            SocketGuildChannel targetChannel = MatchedChannel.FirstOrDefault(x => x.Value == message.Channel as SocketGuildChannel).Key;

            await targetChannel.Guild.GetTextChannel(targetChannel.Id).SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"{message.Author.Username} - {messageContent}", Color.Gold));
        }
    }
    
    public static async Task Matchmaking()
    {
        Log.Debug(nameof(Matchmaking), "Starting matchmaking...");
        
        SocketGuildChannel channel1 = AvaliableGuildChannel[0];
        SocketGuildChannel channel2 = AvaliableGuildChannel[1];
        
        MatchedChannel.Add(channel1, channel2);
        
        AvaliableGuildChannel.Remove(channel1);
        AvaliableGuildChannel.Remove(channel2);

        await channel1.Guild.GetTextChannel(channel1.Id)
            .SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"หาห้องแชทสำเร็จ! เริ่มทำการต่อห้อง", Color.Gold));

        await channel2.Guild.GetTextChannel(channel2.Id).SendMessageAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"หาห้องแชทสำเร็จ! เริ่มทำการต่อห้อง",
                Color.Gold));
        
        Log.Debug(nameof(Matchmaking), "Matchmaking complete.");
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
            channel1 = MatchedChannel.FirstOrDefault(x => x.Value == channel).Key;
            channel2 = channel;
            
            MatchedChannel.Remove(MatchedChannel.FirstOrDefault(x => x.Value == channel).Key);
        }
        
        await channel1.Guild.GetTextChannel(channel1.Id)
            .SendMessageAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"ถูกตัดการเชื่อมต่อ", Color.Gold));

        await channel2.Guild.GetTextChannel(channel2.Id).SendMessageAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", $"ถูกตัดการเชื่อมต่อ",
                Color.Gold));
    }
}