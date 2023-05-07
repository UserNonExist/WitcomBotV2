namespace WitcomBotV2.Command.PingTriggers;

using System.Threading.Tasks;
using WitcomBotV2.Service;
using WitcomBotV2;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

public partial class TriggerCommands
{
    [SlashCommand("remove", "ลบ ping trigger อันปัจจุบันของคุณ")]
    public async Task DoRemoveTrigger()
    {
        DatabaseHandler.RemoveEntry(Context.User.Id, DatabaseType.Ping);
        await RespondAsync("Ping trigger ถูกลบแล้ว", ephemeral: true);
    }
}