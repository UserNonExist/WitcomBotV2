using Discord;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;

public class RoleReaction
{
    public static List<ReactionRoleData> ReactionRoleData { get; set; } = new();
    
    public static void Init()
    {
        Bot.Client.ReactionAdded += HandleReactionAdded;
        Bot.Client.ReactionRemoved += HandleReactionRemoved;

        Refresh();
    }
    

    public static void Refresh()
    {
        try
        {
            ReactionRoleData = DatabaseHandler.GetReactionRoleData();
        
            Log.Debug(nameof(RoleReaction), "ReactionRoleData refreshed.");

            foreach (var data in ReactionRoleData)
            {
                var channel = Bot.Client.GetChannel(ulong.Parse(data.ChannelId)) as ISocketMessageChannel;
                var message = channel.GetMessageAsync(ulong.Parse(data.MessageId)).Result;
                message.AddReactionAsync(Emote.Parse(data.Emote));
            }
        }
        catch (Exception e)
        {
            Log.Error(nameof(Refresh), e);
        }
    }
    
    public static Task HandleReactionAdded(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.UserId == Bot.Client.CurrentUser.Id || reaction.User.Value.IsBot)
            return Task.CompletedTask;
        
        Log.Warn("ReactionAdded", $"Reaction added {reaction.Emote} {reaction.MessageId} {reaction.Channel.Id} {reaction.UserId}");
        
        var data = ReactionRoleData.FirstOrDefault(d => d.MessageId == reaction.MessageId.ToString() && d.ChannelId == reaction.Channel.Id.ToString() && d.Emote == reaction.Emote.ToString());
        
        if (data == null)
            return Task.CompletedTask;
        
        var guildUser = Bot.Instance.Guild.GetUser(reaction.UserId);
        
        var role = guildUser.Guild.GetRole(ulong.Parse(data.RoleId));
        
        guildUser.AddRoleAsync(role);
        
        return Task.CompletedTask;
    }

    private static Task HandleReactionRemoved(Cacheable<IUserMessage, ulong> message, Cacheable<IMessageChannel, ulong> channel, SocketReaction reaction)
    {
        if (reaction.UserId == Bot.Client.CurrentUser.Id || reaction.User.Value.IsBot)
            return Task.CompletedTask;
        
        if (reaction.UserId == Bot.Client.CurrentUser.Id || reaction.User.Value.IsBot)
            return Task.CompletedTask;
        
        Log.Warn("ReactionAdded", $"Reaction added {reaction.Emote} {reaction.MessageId} {reaction.Channel.Id} {reaction.UserId}");
        
        var data = ReactionRoleData.FirstOrDefault(d => d.MessageId == reaction.MessageId.ToString() && d.ChannelId == reaction.Channel.Id.ToString() && d.Emote == reaction.Emote.ToString());
        
        if (data == null)
            return Task.CompletedTask;
        
        var guildUser = Bot.Instance.Guild.GetUser(reaction.UserId);
        
        var role = guildUser.Guild.GetRole(ulong.Parse(data.RoleId));
        
        guildUser.RemoveRoleAsync(role);
        
        return Task.CompletedTask;
    }
}