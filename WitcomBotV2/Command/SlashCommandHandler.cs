using System.Reflection;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Service;


namespace WitcomBotV2.Command;

public class SlashCommandHandler
{
    private readonly InteractionService _service;
    private readonly DiscordSocketClient _client;
    
    public SlashCommandHandler(InteractionService service, DiscordSocketClient client)
    {
        _service = service;
        _client = client;
    }

    public async Task InstallCommandAsync()
    {
        await _service.AddModulesAsync(Assembly.GetEntryAssembly(), null);
        _client.InteractionCreated += HandleInteraction;
        _service.SlashCommandExecuted += HandleSlashCommand;
        _service.ContextCommandExecuted += HandleContextCommand;
        _service.ComponentCommandExecuted += HandleComponentCommand;
    }
    
    private async Task HandleInteraction(SocketInteraction interaction)
    {
        try
        {
            SocketInteractionContext context = new(_client, interaction);
            await _service.ExecuteCommandAsync(context, null);
            //await SpamPrevention.HandleInteraction(interaction);
            Log.Info(nameof(HandleInteraction), $"{interaction.User.Username} used an interaction.");
        }
        catch (Exception e)
        {
            Log.Error(nameof(HandleInteraction), e);
            if (interaction.Type == InteractionType.ApplicationCommand)
                await interaction.RespondAsync(
                    embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, e.Message));
        }
    }

    private async Task HandleServiceError(IInteractionContext context, IResult result)
    {
        if (!result.IsSuccess)
        {
            if (result.Error == InteractionCommandError.UnknownCommand)
                return;

            await context.Interaction.RespondAsync(
                embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, result.ErrorReason));
        }
    }

    private async Task HandleSlashCommand(SlashCommandInfo info, IInteractionContext context, IResult result) =>
        await HandleServiceError(context, result);

    private async Task HandleContextCommand(ContextCommandInfo info, IInteractionContext context, IResult result) =>
        await HandleServiceError(context, result);

    private async Task HandleComponentCommand(ComponentCommandInfo info, IInteractionContext context, IResult result) =>
        await HandleServiceError(context, result);
}