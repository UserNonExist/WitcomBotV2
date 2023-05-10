namespace WitcomBotV2.Command.PingTriggers;

using System.Threading.Tasks;
using WitcomBotV2.Service;
using Discord.Commands;
using Discord.Interactions;

using Group = Discord.Interactions.GroupAttribute;
using Summary = Discord.Interactions.SummaryAttribute;

[Group("pt", "คำสั่งจัดการกับ ping triggers.")]
public partial class TriggerCommands : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("add", "เพิ่มข้อความ ping trigger.")]
    public async Task AddPingTrigger([Summary("Message", "ข้อความที่จะส่ง")] [Remainder] string message)
    {
        if (message.Length > Program.Config.TriggerLengthLimit)
        {
            await RespondAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.TriggerLengthExceedsLimit, Program.Config.TriggerLengthLimit.ToString()), ephemeral: true);
            return;
        }

        bool flag = false;
        if (!string.IsNullOrEmpty(DatabaseHandler.GetPingTrigger(Context.User.Id)))
        {
            DatabaseHandler.RemoveEntry(Context.User.Id, DatabaseType.Ping);
            flag = true;
        }

        DatabaseHandler.AddEntry(Context.User.Id, message, DatabaseType.Ping);
        await RespondAsync($"Ping trigger {(flag ? "ถูกเปลี่ยนแล้ว" : "ถูกเพิ่มแล้ว")}.", ephemeral: true);
    }
}