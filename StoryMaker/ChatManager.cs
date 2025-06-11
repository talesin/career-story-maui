using Microsoft.Extensions.Logging;
using OpenAI;
using OpenAI.Chat;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Nodes;
using System.Threading.Tasks;

namespace StoryMaker
{
    public abstract record ChatMessage(string Content);
    public record SystemMessage(string Content) : ChatMessage(Content);
    public record UserMessage(string Content) : ChatMessage(Content);

    public enum ChatResponseType
    {
        Text,
        //Image,
        //Refusal
    }
    public record ChatResponseItem(string Text, ChatResponseType ResponseType);

    public record ChatResponse(List<ChatResponseItem> Items);

    public interface IChatManager
    {
        IChatClient GetChatClient(string modelName);
    }
    public record JsonSchema(JsonNode Definition, string Title, bool IsStrict);

    public interface IChatClient
    {
        Task<ChatResponse> GetFormattedResponseAsync(IEnumerable<ChatMessage> message, JsonSchema schema);
    }


    public class OpenAIChatManager(OpenAIClient client, ILogger logger) : IChatManager
    {
        private readonly OpenAIClient client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public IChatClient GetChatClient(string modelName)
        {
            return new OpenAIChatClient(client.GetChatClient(modelName), logger);
        }
    }

    public class OpenAIChatClient(ChatClient client, ILogger logger) : IChatClient
    {
        private readonly ChatClient client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

        public async Task<ChatResponse> GetFormattedResponseAsync(IEnumerable<ChatMessage> messages, JsonSchema schema)
        {
            ChatCompletionOptions options = new()
            {
                ResponseFormat = ChatResponseFormat.CreateJsonSchemaFormat(
                    jsonSchemaFormatName: schema.Title,
                    jsonSchema: BinaryData.FromString(schema.Definition.ToString()),
                    jsonSchemaIsStrict: schema.IsStrict),
            };


            var chatMessages = messages.Select(m => m switch
            {
                SystemMessage sm => new SystemChatMessage(sm.Content) as OpenAI.Chat.ChatMessage,
                UserMessage um => new UserChatMessage(um.Content),
                _ => throw new InvalidOperationException("Unsupported message type")
            });

            var result = await client.CompleteChatAsync(chatMessages, options, CancellationToken.None);
            var items = result.Value.Content.Select(c => new ChatResponseItem(c.Text, ChatResponseType.Text)).ToList();

            return new ChatResponse(items);
        }
    }
}
