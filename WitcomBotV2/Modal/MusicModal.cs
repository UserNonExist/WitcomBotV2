using Discord;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Modal;

public class MusicModal
{
    public static ButtonBuilder PreviousButton { get; } = new("Previous", "music-previous", ButtonStyle.Secondary);
    public static ButtonBuilder NextButton { get; } = new("Next", "music-next", ButtonStyle.Secondary);

    public static async Task<MessageComponent> CreateButton()
    {
        var button = new ComponentBuilder().WithButton(PreviousButton).WithButton(NextButton).Build();
        return button;
    }
    
    public static Dictionary<SocketUserMessage, List<Embed>> _pages = new();
    
    public static async Task HandleButton(SocketMessageComponent component)
    {
        Log.Debug(nameof(HandleButton), $"Handling button {component.Data.CustomId} from {component.User.Username}#{component.User.Discriminator} ({component.User.Id})");
        
        if (component.Data.CustomId == PreviousButton.CustomId)
        {
            Log.Debug(nameof(HandleButton), $"Previous button clicked by {component.User.Username}#{component.User.Discriminator} ({component.User.Id})");
            
            
        }
        
        if (component.Data.CustomId == NextButton.CustomId)
        {
            Log.Debug(nameof(HandleButton), $"Next button clicked by {component.User.Username}#{component.User.Discriminator} ({component.User.Id})");
        }
    }
}