using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command.SecureChat;

[Group ("secure", "คำสั่งเกี่ยวกับการสนทนาข้ามเซิร์ฟเวอร์")]
public partial class SecureCommand : InteractionModuleBase<ShardedInteractionContext>
{
    public Dictionary<ulong, ulong> GuildMessage = new();
    
    [SlashCommand("connect", "เชื่อมต่อกับห้องสนทนาข้ามเซิร์ฟเวอร์")]
    public async Task Connect()
    {
        if (Module.SecureChatModule.MatchedChannel.ContainsKey(Context.Guild.Channels.FirstOrDefault(x => x.Id == Context.Channel.Id)) || Module.SecureChatModule.MatchedChannel.ContainsValue(Context.Guild.Channels.FirstOrDefault(x => x.Id == Context.Channel.Id)))
        {
            await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", "คุณได้เชื่อมต่อกับห้องสนทนาข้ามเซิร์ฟเวอร์ไปแล้ว", Color.Red), ephemeral: true);
            return;
        }
        
        Module.SecureChatModule.AvaliableGuildChannel.Add(Context.Guild.Channels.FirstOrDefault(x => x.Id == Context.Channel.Id));
        
        await RespondAsync(embed: await EmbedBuilderService.CreateBasicEmbed("Secure Chat", "เริ่มการค้นหาห้องสนทนาข้ามเซิร์ฟเวอร์", Color.Blue), ephemeral: true);
        
        if (Module.SecureChatModule.AvaliableGuildChannel.Count >= 2)
        {
            await Module.SecureChatModule.Matchmaking();
        }
    }
}