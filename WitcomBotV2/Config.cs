﻿namespace WitcomBotV2;

public class Config
{
    public string BotPrefix { get; set; }
    public string BotToken { get; set; }
    public ulong GuildId { get; set; }
    public bool Debug { get; set; }
    public ulong DiscAdminId { get; set; }
    public int TriggerLengthLimit { get; set; }
    public ulong ChannelRentId { get; set; }
    public ulong ChannelRentCatId { get; set; }
    public string OpenAIAPIKey { get; set; }
    public string LLRESTUri { get; set; }
    public string LLWebSocketUri { get; set; }
    public string LLPassword { get; set; }
    public string UnloadPassword { get; set; }
    public int ShardId { get; set; }
    public int TotalShards { get; set; }
}