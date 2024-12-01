using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Service;
namespace WitcomBotV2.Command.RoleReact;

[Group("react", "คำสั่งจัดการกับ reaction role")]
public partial class ReactCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("add", "เพิ่ม react role (ALL INPUT AS ID EXCEPT EMOTE)")]
    public async Task Add(IChannel channel, string message, string role, string emote)
    {
        
        SocketGuildUser guildUser = (SocketGuildUser)Context.User;
        var permissions = guildUser.Roles;

        if (permissions.All(r => r.Id != Program.Config.DiscAdminId))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }
        
        Log.Warn(nameof(ReactCommand), $"Adding reaction role {role} {message} {emote}");
            
        DatabaseHandler.AddReactionRoleData(ulong.Parse(role), ulong.Parse(message), emote, channel.Id);
        
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Success", "Reaction role added", Color.Green), ephemeral: true);
    }
}