namespace WitcomBotV2.Command;

using System.Text;
using WitcomBotV2.Service;
using Discord;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public class HelpCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("help", "บอกคำสั่งทั้งหมด")]
    public async Task Help()
    {
        EmbedBuilder builder = new();
        builder.WithColor(Color.Purple);
        builder.WithCurrentTimestamp();
        builder.WithTitle("Commands");
        builder.WithFooter(EmbedBuilderService.FooterText);

        int count = 0;
        foreach (SlashCommandInfo command in Bot.Instance.InteractionService.SlashCommands)
        {
            if (command.Description == "DNI")
                continue;
            count++;
            List<string> args = new();
            foreach (SlashCommandParameterInfo arg in command.Parameters)
                args.Add($"{arg.Name} -- {arg.Description}");

            StringBuilder stringBuilder = new();
            foreach (string s in args)
                if (!string.IsNullOrEmpty(s))
                    stringBuilder.AppendLine(s);
            stringBuilder.AppendLine(command.Description ?? "No description");
            builder.AddField(command.Name, stringBuilder.ToString());

            if (count == 24)
            {
                await RespondAsync(embed: builder.Build());
                builder = new();
                builder.WithColor(Color.Purple);
                builder.WithCurrentTimestamp();
                builder.WithTitle("Commands Contd.");
                builder.WithFooter(EmbedBuilderService.FooterText);
                count = 0;
            }
        }

        await RespondAsync(embed: builder.Build(), ephemeral: false);
    }
}