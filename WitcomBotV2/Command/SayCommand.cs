using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;

public class SayCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("say", "DNI")]
    public async Task Say([Discord.Interactions.Summary("Channel", "ห้องที่จะส่ง")] IChannel channel,[Discord.Interactions.Summary("Message", "ข้อความที่จะส่ง")] [Remainder] string message)
    {
        SocketGuildUser guildUser = (SocketGuildUser)Context.User;
        var permissions = guildUser.Roles;

        if (permissions.Any(r => r.Id == Program.Config.DiscAdminId))
        {
            if (channel == null)
                channel = Context.Channel;
            
            if (message == null)
            {
                await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Error", "กรุณาใส่ข้อความ", Color.Red), ephemeral: true);
                return;
            }
            
            try
            {
                await Bot.Client.GetGuild(Context.Guild.Id).GetTextChannel(channel.Id).SendMessageAsync(message);
            }
            catch (Exception e)
            {
                await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Error", "ไม่สามารถส่งข้อความได้", Color.Red), ephemeral: true);
                Log.Error(nameof(Say), e);
                throw;
            }
        
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Success", $"ส่งข้อความแล้ว", Color.Green), ephemeral: true);
        }
        else
        {
            await RespondAsync(
                embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied),
                ephemeral: true);
        }
    }
    
}