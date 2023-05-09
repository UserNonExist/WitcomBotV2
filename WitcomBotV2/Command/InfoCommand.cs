using System.Reflection;
using Discord;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;

public class InfoCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [SlashCommand("info", "ดูข้อมูลเกี่ยวกับบอท")]
    public async Task Info()
    {
        string message =
            "WitcomBotV2 เขียนใหม่แทนอันเก่าที่ unoptimized\n\n" +
            "สามารถช่วยพัฒนาได้ที่ https://github.com/UserNonExist/WitcomBotV2\n" +
            "เวอร์ชั่นปัจจุบัน: " + Assembly.GetExecutingAssembly().GetName().Version;

        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Info", message, Color.Gold),
            ephemeral: false);
    }
}