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
    /// <summary>
    /// Represents a message in a chat conversation.
    /// </summary>
    public abstract record ChatMessage(string Content);

    /// <summary>
    /// Represents a system message in a chat conversation.
    /// </summary>
    public record SystemMessage(string Content) : ChatMessage(Content);

    /// <summary>
    /// Represents a user message in a chat conversation.
    /// </summary>
    public record UserMessage(string Content) : ChatMessage(Content);

    /// <summary>
    /// Types of chat responses.
    /// </summary>
    public enum ChatResponseType
    {
        Text,
        //Image,
        //Refusal
    }

    /// <summary>
    /// Represents a single item in a chat response.
    /// </summary>
    public record ChatResponseItem(string Text, ChatResponseType ResponseType);

    /// <summary>
    /// Represents a chat response containing multiple items.
    /// </summary>
    public record ChatResponse(List<ChatResponseItem> Items);

    /// <summary>
    /// Interface for managing chat clients.
    /// </summary>
    public interface IChatManager
    {
        /// <summary>
        /// Gets a chat client for the specified model name.
        /// </summary>
        /// <param name="modelName">The name of the model.</param>
        /// <returns>An IChatClient instance.</returns>
        IChatClient GetChatClient(string modelName);
    }

    /// <summary>
    /// Represents a JSON schema definition for chat responses.
    /// </summary>
    public record JsonSchema(JsonNode Definition, string Title, bool IsStrict);

    /// <summary>
    /// Interface for a chat client that can get formatted responses.
    /// </summary>
    public interface IChatClient
    {
        /// <summary>
        /// Gets a formatted response from the chat client based on the provided messages and schema.
        /// </summary>
        /// <param name="message">The chat messages.</param>
        /// <param name="schema">The JSON schema to use for formatting.</param>
        /// <returns>A ChatResponse object.</returns>
        Task<ChatResponse> GetFormattedResponseAsync(IEnumerable<ChatMessage> message, JsonSchema schema);
    }

    /// <summary>
    /// Manages OpenAI chat clients.
    /// </summary>
    public class OpenAIChatManager(OpenAIClient client, ILogger<OpenAIChatManager> logger) : IChatManager
    {
        private readonly OpenAIClient client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Gets an OpenAI chat client for the specified model name.
        /// </summary>
        /// <param name="modelName">The name of the model.</param>
        /// <returns>An IChatClient instance.</returns>
        public IChatClient GetChatClient(string modelName)
        {
            return new OpenAIChatClient(client.GetChatClient(modelName), logger);
        }
    }

    /// <summary>
    /// Represents an OpenAI chat client that can get formatted responses.
    /// </summary>
    public class OpenAIChatClient(ChatClient client, ILogger logger) : IChatClient
    {
        private readonly ChatClient client = client ?? throw new ArgumentNullException(nameof(client));
        private readonly ILogger logger = logger ?? throw new ArgumentNullException(nameof(logger));

        /// <summary>
        /// Gets a formatted response from the OpenAI chat client based on the provided messages and schema.
        /// </summary>
        /// <param name="messages">The chat messages.</param>
        /// <param name="schema">The JSON schema to use for formatting.</param>
        /// <returns>A ChatResponse object.</returns>
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
