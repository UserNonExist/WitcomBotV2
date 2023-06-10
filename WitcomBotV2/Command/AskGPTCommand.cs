using Discord;
using Discord.Commands;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;

public class AskGPTCommand : InteractionModuleBase<ShardedInteractionContext>
{
    [DefaultMemberPermissions(GuildPermission.SendMessages)]
    [SlashCommand("askgpt", "DNI ถาม GPT-3")]
    public async Task AskGPT([Discord.Interactions.Summary("Question", "คำถาม")] [Remainder] string question)
    {
        // await DeferAsync();
        // try
        // {
        //     await FollowupAsync(embed: await EmbedBuilderService.CreateBasicEmbed("GPT-3", GPTModule.AskGPT3(question).Result, Color.Blue));
        // }
        // catch (Exception e)
        // {
        //     await FollowupAsync(embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.Unspecified, e.Message));
        // }

        await RespondAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("GPT-3",
                "ตอนนี้ระบบ AskGPT ถูกปิดใช้งานเนื่องจากว่าเจ้าของบอทไม่มีเงินจ่ายค่า API\n\nขออภัยในความไม่สะดวก",
                Color.Purple), ephemeral: true);
    }
}