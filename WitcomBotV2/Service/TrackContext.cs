using Discord.WebSocket;

namespace WitcomBotV2.Service;

public sealed class TrackContext
{
    public SocketUser Requester { get; set; }
    public SocketGuild Guild { get; set; }
    public SocketTextChannel Channel { get; set; }
}