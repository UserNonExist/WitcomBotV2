using Discord;
using Discord.Commands;
using Discord.Interactions;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.ChannelRenting;

[Discord.Interactions.Group("rent", "คำสั่งเพื่อควบคุมการเช่าห้อง VC")]
public partial class RentCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("deny", "ห้ามคนที่เลือกไม่ให้เข้า VC คุณ")]
    public async Task Deny([Discord.Interactions.Summary("Users", "คนที่จะไม่สามารถเข้าได้")] [Remainder] string users)
    {
        if (!Module.ChannelRenting.IsRenting(Context.User))
        {
            await RespondAsync(
                embed: await ErrorHandlingService.GetErrorEmbed(ErrorCodes.PermissionDenied,
                    "คุณสามารถใช้คำสั่งนี้ได้ในขณะที่คุณเป็นคนเช่า VC เท่านั้น"), ephemeral: true);
            return;
        }

        List<IGuildUser> guildUsers = new();

        foreach (string s in users.Split(' '))
        {
            if (ulong.TryParse(s.Replace("<", string.Empty).Replace("@", string.Empty).Replace(">", string.Empty), out ulong userId))
            {
                IGuildUser user = Context.Guild.GetUser(userId);
                if (user is not null)
                    guildUsers.Add(user);
            }
        }

        IVoiceChannel channel = Bot.Instance.Guild.GetVoiceChannel(Module.ChannelRenting.RentedChannels[Context.User]);

        foreach (IGuildUser user in guildUsers)
            await channel.AddPermissionOverwriteAsync(user, new OverwritePermissions(connect: PermValue.Deny));

        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("VC Renting",
            "ผู้ใช้ที่เลือกไว้ถูกปฏิเสธไม่ให้เข้าห้อง VC คุณ", Color.Magenta));
    }
}