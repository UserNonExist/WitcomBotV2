using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.RoleReact;

public partial class ReactCommand
{
    [SlashCommand("remove", "delete reaction role")]
    public async Task Remove(int id)
    {
        SocketGuildUser guildUser = (SocketGuildUser)Context.User;
        var permissions = guildUser.Roles;

        if (permissions.All(r => r.Id != Program.Config.DiscAdminId))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }
        
        if (RoleReaction.ReactionRoleData.All(r => r.Id != id))
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Error", "Data not found", Color.Red), ephemeral: true);
            return;
        }

        DatabaseHandler.RemoveEntry(id, DatabaseType.ReactRole);
        await RespondAsync("Removed", ephemeral: true);
    }
}