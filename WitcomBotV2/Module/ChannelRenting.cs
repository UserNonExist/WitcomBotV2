using Discord;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;

public class ChannelRenting
{
    public static Dictionary<SocketUser, ulong> RentedChannels { get; } = new();
    public static bool IsRenting(SocketUser user) => RentedChannels.ContainsKey(user);
    public static bool IsRented(ulong channel) => RentedChannels.ContainsValue(channel);

    private static List<string> ChannelNames { get; } = new()
    {
        "ห้องของ %user",
        "%user's hellhole",
        "ห้องใต้ดินของ %user",
        "ที่นอนเล่นของ %user"
    };

    public static async Task OnVoiceStateChanged(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (user.IsBot)
            return;

        if (after.VoiceChannel != null && after.VoiceChannel.Id == Program.Config.ChannelRentId)
            await HandleUserJoined(user, before, after);
        else if (before.VoiceChannel != null && IsRented(before.VoiceChannel.Id))
            await HandleUserLeft(user, before, after);
    }
    
    private static async Task HandleUserJoined(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        IGuildUser guildUser = (IGuildUser) user;
        if (RentedChannels.ContainsKey(user))
        {
            Log.Debug($"{nameof(ChannelRenting)}.{nameof(HandleUserJoined)}","User already has a rented channel.");
            return;
        }

        if (after.VoiceChannel.Id != Program.Config.ChannelRentId)
        {
            Log.Error($"{nameof(ChannelRenting)}.{nameof(HandleUserJoined)}","User joined non-rent channel.");
            return;
        }

        int r = Program.Rng.Next(ChannelNames.Count);
        string userName = guildUser.Nickname != null && !string.IsNullOrEmpty((guildUser.Nickname))
            ? guildUser.Nickname
            : guildUser.Username;
        string channelName = ChannelNames[r].Replace("%user", userName);

        IGuild guild = after.VoiceChannel.Guild;
        if (guild == null)
        {
            Log.Error($"{nameof(ChannelRenting)}.{nameof(HandleUserJoined)}","Guild is null, What the fuck.");
            return;
        }

        var channel = await guild.CreateVoiceChannelAsync(channelName, properties =>
        {
            properties.UserLimit = 12;
            properties.Name = channelName;
            properties.CategoryId = Program.Config.ChannelRentCatId;
        }, RequestOptions.Default);

        await channel.AddPermissionOverwriteAsync(guild.EveryoneRole,
            new OverwritePermissions(connect: PermValue.Allow));
        await channel.AddPermissionOverwriteAsync(guildUser, new(connect: PermValue.Allow));
        var staffRole = guild.GetRole(Program.Config.DiscAdminId);
        await channel.AddPermissionOverwriteAsync(staffRole,
            new OverwritePermissions(connect: PermValue.Allow, manageChannel: PermValue.Allow));

        await guildUser.ModifyAsync(x => x.ChannelId = channel.Id);
        
        RentedChannels.Add(user, channel.Id);
        Log.Debug($"{nameof(ChannelRenting)}.{nameof(HandleUserJoined)}","Created rent channel suscessfully");
    }

    private static async Task HandleUserLeft(SocketUser user, SocketVoiceState before, SocketVoiceState after)
    {
        if (before.VoiceChannel.ConnectedUsers.Count == 0)
        {
            await before.VoiceChannel.DeleteAsync();
            RentedChannels.Remove(user);
            Log.Debug($"{nameof(ChannelRenting)}.{nameof(HandleUserLeft)}","Deleted rent channel suscessfully");
        }
    }
}