using Discord;
using Discord.Commands;
using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;


public class RunCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("run", "Run a command")]
    public async Task Run([Discord.Interactions.Summary("Command", "Command to run")] string command)
    {
        SocketGuildUser guildUser = (SocketGuildUser)Context.User;
        
        if (guildUser.Id != 315717809395204098)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied), ephemeral: true);
            return;
        }

        await DeferAsync(ephemeral: true);
        
        var args = command.Split(' ');
        
        bool success = false;
        
        switch(args[0].ToLower())
        {
            case "reload":
                success = await Program.ReloadConfig();
                if (success)
                {
                    await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Run", "Config reloaded", Color.Green), ephemeral: true);
                }
                else
                {
                    await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Run", "Failed to reload config", Color.Red), ephemeral: true);
                }
                break;
            case "shutdown":
                await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Run", "Shutting down", Color.Green), ephemeral: true);
                await Bot.Client.LogoutAsync();
                AppDomain.CurrentDomain.ProcessExit += (s, e) => Environment.Exit(0);
                break;
            case "removeplaytime":
                if (args.Length < 2)
                {
                    await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Run", "Please provide a srv-server", Color.Red), ephemeral: true);
                    return;
                }
                
                var mcInfo = Bot.Instance.MinecraftModule.MinecraftServerInfos.Find(x => x.SrvRecord == args[1]);
                
                if (mcInfo == null)
                {
                    await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Run", "Server not found", Color.Red), ephemeral: true);
                    return;
                }

                success = await Bot.Instance.MinecraftModule.RemoveAllPlaytime(mcInfo);
                
                if (success)
                {
                    await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Run", "Playtime removed", Color.Green), ephemeral: true);
                }
                else
                {
                    await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Run", "Failed to remove playtime", Color.Red), ephemeral: true);
                }
                break;
            case "forceexplode":
                BombModule.ForceHit = true;
                await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Run", "Next message will trigger the bomb module", Color.Green), ephemeral: true);
                break;
            default:
                await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Run", "Command not found", Color.Red), ephemeral: true);
                break;
        }
    }
}