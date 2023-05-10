using Discord;
using Discord.WebSocket;

namespace WitcomBotV2.Modal;

public class MusicModal
{
    public static ButtonBuilder PreviousButton { get; } = new("Pervious", "music-pervious", ButtonStyle.Secondary);
    public static ButtonBuilder NextButton { get; } = new("Next", "music-next", ButtonStyle.Secondary);

    public static async Task<MessageComponent> CreateButton()
    {
        var button = new ComponentBuilder().WithButton(PreviousButton).WithButton(NextButton).Build();
        return button;
    }
    
    //public static Dictionary<RestInteractionMessage, List<Embed>> _pages = new();
}