using Discord;
using OpenAI_API;
using WitcomBotV2.Service;

namespace WitcomBotV2.Module;

public class GPTModule
{
    private static OpenAIAPI _openAiApi;
    
    public static async Task Init()
    {
        _openAiApi = new OpenAIAPI(Program.Config.OpenAIAPIKey);
    }
    
    public static async Task<string> AskGPT3(string question)
    {
        var conversation = _openAiApi.Chat.CreateConversation();
        conversation.AppendUserInput(question);

        return $"Question: {question}\n\nAnswer: " + await conversation.GetResponseFromChatbotAsync();;
    }
}