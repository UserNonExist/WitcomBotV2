

namespace WitcomBotV2.Command;

using System.Reflection;
using Discord;
using Discord.Commands;
using Discord.WebSocket;
using Service;
using WitcomBotV2.TypeReaders;

[Obsolete("This class is deprecated, use InteractionModuleBase instead.")]
public class CommandHandler
{
    private readonly DiscordShardedClient client;
    private readonly CommandService service;

    public CommandHandler(DiscordShardedClient client, CommandService service)
    {
        this.client = client;
        this.service = service;
    }

    public async Task InstallCommandsAsync()
    {
        client.MessageReceived += HandleCommandAsync;
        service.AddTypeReader(typeof(IEmote), new EmoteTypeReader());
        await service.AddModulesAsync(Assembly.GetExecutingAssembly(), null);
    }

    private async Task HandleCommandAsync(SocketMessage message)
    {
        if (message is not SocketUserMessage msg)
            return;

        int argPos = 0;
        if (!(msg.HasStringPrefix(Program.Config.BotPrefix, ref argPos) ||
              msg.HasMentionPrefix(client.CurrentUser, ref argPos)) || msg.Author.IsBot)
            return;

        ShardedCommandContext context = new(client, msg);

        try
        {
            await service.ExecuteAsync(context, argPos, null);
        }
        catch (Exception e)
        {
            Log.Error(nameof(HandleCommandAsync), $"Error executing command: {message.Content}\n{e}");
            await message.Channel.SendMessageAsync(
                embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, e.Message));
        }
    }

    public static bool CanRunStaffCmd(SocketUser user, bool allowContributors = false) => CanRunStaffCmd((IGuildUser)user, allowContributors);

    public static bool CanRunStaffCmd(IGuildUser user, bool allowContributors = false) => user.RoleIds.Any(roleId => roleId == Program.Config.DiscAdminId || user.GuildPermissions.Administrator || user.GuildPermissions.ManageChannels);
}