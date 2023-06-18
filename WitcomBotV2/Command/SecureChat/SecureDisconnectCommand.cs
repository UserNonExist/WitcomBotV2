using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.SecureChat;

public partial class SecureCommand
{
    [SlashCommand("disconnect", "ยกเลิกการเชื่อมต่อกับห้องสนทนาข้ามเซิร์ฟเวอร์")]
    public async Task Disconnect()
    {
        if (Module.SecureChatModule.AvaliableGuildChannel.Contains(
                Context.Guild.Channels.FirstOrDefault(x => x.Id == Context.Channel.Id)))
        {
            Module.SecureChatModule.AvaliableGuildChannel.Remove(
                Context.Guild.Channels.FirstOrDefault(x => x.Id == Context.Channel.Id));
            
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", "ยกเลิกการค้นหาห้องสนทนาข้ามเซิร์ฟเวอร์", Color.Blue), ephemeral: true);
            return;
        }
        
        if (Module.SecureChatModule.MatchedChannel.ContainsKey(
                Context.Guild.Channels.FirstOrDefault(x => x.Id == Context.Channel.Id)) ||
            Module.SecureChatModule.MatchedChannel.ContainsValue(
                Context.Guild.Channels.FirstOrDefault(x => x.Id == Context.Channel.Id)))
        {
            
            Module.SecureChatModule.DestroyLobby(Context.Guild.Channels.FirstOrDefault(x => x.Id == Context.Channel.Id));
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", "ทำการตักการเชื่อมต่อกับห้องแชท", Color.Red), ephemeral: true);
            return;
        }
        
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", "คุณยังไม่ได้เชื่อมต่อกับห้องสนทนาข้ามเซิร์ฟเวอร์", Color.Red), ephemeral: true);
    }
}