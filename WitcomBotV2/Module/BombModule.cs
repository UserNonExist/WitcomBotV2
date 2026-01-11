using Discord;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;

public class BombModule
{
    public static bool ForceHit = false;
    
    public static async Task MessageReceived(SocketMessage msg)
    {
        var guild = msg.Channel as IGuildChannel;

        if (guild is null)
            return;
        
        if (msg.Author.IsBot || !Program.Config.BombModuleGuildId.Contains(guild.GuildId))
            return;

        try
        {
            var random = new Random();

            var chance = random.Next(0, 100);
            
            Log.Debug(nameof(MessageReceived), $"{msg.Author.Mention}: {chance}%");

            if (chance <= Program.Config.BombModuleHitChance || ForceHit)
            {
                ForceHit = false;

                var person = msg.Author as IGuildUser;
                
                if  (person is null)
                {
                    Log.Warn(nameof(MessageReceived), "No person found");
                    return;
                }

                try
                {
                    await person.SetTimeOutAsync(TimeSpan.FromMinutes(1));
                    
                    Log.Info(nameof(MessageReceived), $"Timed out: {msg.Author.GlobalName}");
                    await msg.Channel.SendMessageAsync(
                        "%user stepped on a landmine and 💥💥💥 (1 minute timeout)".Replace("%user", msg.Author.Mention));
                }
                catch
                {
                    Log.Error(nameof(MessageReceived), $"Failed to timeout user. ({person.DisplayName}, {person.Guild.Name})");
                }
            }
        }
        catch (Exception e)
        {
            Log.Error(nameof(MessageReceived), e);
        }
    }
}