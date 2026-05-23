using Microsoft.Extensions.AI;
using OpenAI;
using OpenAI.Chat;
using System.ClientModel;
using ChatMessage = Microsoft.Extensions.AI.ChatMessage;

namespace Wolfish.ChatAgent
{
    public class OpenAiAgent
    {
        public OpenAI.Chat.ChatClient _openAiClient;
        private IChatClient _chatClient;

        public OpenAiAgent(string model, string endpoint, string apiKey)
        {
            var options = new OpenAIClientOptions { Endpoint = new Uri(endpoint) };

            if (string.IsNullOrEmpty(apiKey))
            {
                _openAiClient = new ChatClient(model, new ApiKeyCredential("*"), options);
            }
            else
            {
                _openAiClient = new ChatClient(model, new ApiKeyCredential(apiKey), options);
            }

            _chatClient = _openAiClient.AsIChatClient();
        }

        /// <summary>
        /// Envia a mensagem do usuário ao agente e recebe a resposta em string.
        /// </summary>
        public async Task<string> SendMessageAsync(string userMessage)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, userMessage)
            };
            var resposta = await _chatClient.GetResponseAsync(messages);

            var fullResponse = new System.Text.StringBuilder();
            foreach (var message in resposta.Messages)
            {
                fullResponse.Append(message);
            }

            return fullResponse.ToString();
        }

        /// <summary>
        /// Envia a mensagem do usuário ao agente e recebe a resposta em streaming.
        /// </summary>
        public async IAsyncEnumerable<string> SendMessageStreamingAsync(string userMessage)
        {
            var messages = new List<ChatMessage>
            {
                new ChatMessage(ChatRole.User, userMessage)
            };

            await foreach (var chunk in _chatClient.GetStreamingResponseAsync(messages))
            {
                var text = chunk.Text;
                if (!string.IsNullOrEmpty(text))
                {
                    yield return text;
                }
            }
        }
    }
}
