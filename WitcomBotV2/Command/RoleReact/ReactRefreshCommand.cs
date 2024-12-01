using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.RoleReact;

public partial class ReactCommand
{
    [SlashCommand("refresh", "รีเฟรช reaction role")]
    public async Task Refresh()
    {
        SocketGuildUser guildUser = (SocketGuildUser)Context.User;
        var permissions = guildUser.Roles;

        if (permissions.All(r => r.Id != Program.Config.DiscAdminId))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        RoleReaction.Refresh();
        await RespondAsync("Refreshed", ephemeral: true);
    }
}