﻿using Discord;
using Discord.Commands;
using Discord.Interactions;
using WitcomBotV2.Module;
using WitcomBotV2.Service;

namespace WitcomBotV2.Command;

public class AskGPTCommand : InteractionModuleBase<SocketInteractionContext>
{
    [SlashCommand("askgpt", "ถาม GPT-3")]
    public async Task AskGPT([Discord.Interactions.Summary("Question", "คำถาม")] [Remainder] string question)
    {
        string txt = GPTModule.AskGPT3(question).Result;
        await RespondAsync(
            embed: await EmbedBuilderService.CreateBasicEmbed("GPT-3", txt , Color.Blue));
    }
}