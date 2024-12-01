using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.RoleReact;

public partial class ReactCommand
{
    [SlashCommand("list", "list all reaction roles")]
    public async Task List()
    {
        SocketGuildUser guildUser = (SocketGuildUser)Context.User;
        var permissions = guildUser.Roles;

        if (permissions.All(r => r.Id != Program.Config.DiscAdminId))
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle("Reaction Roles")
            .WithColor(Color.Blue)
            .WithFooter(EmbedBuilderService.FooterText);

        foreach (var data in RoleReaction.ReactionRoleData)
        {
            var channel = Bot.Client.GetGuild(Context.Guild.Id).GetTextChannel(ulong.Parse(data.ChannelId));
            var message = await channel.GetMessageAsync(ulong.Parse(data.MessageId));
            var role = Bot.Client.GetGuild(Context.Guild.Id).GetRole(ulong.Parse(data.RoleId));

            embed.Description += $"Id:{data.Id} Message: {message.GetJumpUrl()} Role: {role.Name} Emote: {data.Emote}\n";
        }
        
        await RespondAsync(embed: embed.Build());
    }
}