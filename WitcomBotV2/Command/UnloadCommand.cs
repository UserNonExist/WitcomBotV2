using Discord;
using Discord.Interactions;
using Discord.Rest;

namespace WitcomBotV2.Command;

public class UnloadCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("unloadslash", "DNI")]
    public async Task unload_guild_slash(string password)
    {
        if (password != Program.Config.UnloadPassword)
        {
            await RespondAsync("wrong password", ephemeral: true);
            return;
        }
        
        List<ApplicationCommandProperties> applicationCommandProperties = new List<ApplicationCommandProperties>();
        await Context.Guild.BulkOverwriteApplicationCommandAsync(applicationCommandProperties.ToArray());
        await RespondAsync("guild command unloaded", ephemeral: true);
    }
}