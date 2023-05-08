using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.PingTriggers;

public class SendCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("send", "ส่งข้อความไปหาคนอื่น")]
    public async Task Send([Discord.Interactions.Summary("User", "ผู้ใช้")] SocketUser user, [Discord.Interactions.Summary("Message", "ข้อความ")] [Remainder] string message)
    {
        try
        {
            await user.SendMessageAsync($"ข้อความจากใครไม่รู้: {message}");
        }
        catch (Exception e)
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Error", "ไม่สามารถส่งข้อความได้", Color.Red), ephemeral: true);
            throw;
        }
        
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Success", $"ส่งข้อความไปหา {user.Mention} แล้ว", Color.Green), ephemeral: true);
    }
}