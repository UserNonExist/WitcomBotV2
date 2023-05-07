using System.Reflection;
using Discord;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;

public class InfoCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("info", "ดูข้อมูลเกี่ยวกับบอท")]
    public async Task Info()
    {
        string message =
            "WitcomBotV2 สร้างโดย User_NotExist ซึ่งเขียนใหม่แทนอันเก่าที่ unoptimized\n\n" +
            "สามารถ contribute ได้ที่ https://github.com/UserNonExist/WitcomBotV2\n" +
            "เวอร์ชั่นปัจจุบัน: " + Assembly.GetExecutingAssembly().GetName().Version;

        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Info", message, Color.Gold),
            ephemeral: false);
    }
}