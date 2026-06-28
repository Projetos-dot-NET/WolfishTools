using System.Text.Json;
using Microsoft.Extensions.AI;

public class AgentHistory
{
    public List<ChatMessage> Messages{get; private set;}

    public List<HistoryMessage> _historyMessages;

    private readonly string _path;

    public AgentHistory(string path)
    {
        _path = path;
        Messages = [];
        _historyMessages = [];
    }

    public void AddSystem(string systemMessage)
    {
        var isNotNullOrNotWhitespace = !string.IsNullOrWhiteSpace(systemMessage);
        var isNotDuplicate = !_historyMessages.Any(m => m.Role == "system" && m.Content == systemMessage);

        if (isNotNullOrNotWhitespace && isNotDuplicate) 
        {
            //Messages.Add(new ChatMessage(ChatRole.System, systemMessage));
            _historyMessages.Add(new HistoryMessage { Role = ChatRole.System.ToString(), Content = systemMessage, CreatedAt = DateTimeOffset.UtcNow });
        }
    }

    public void AddUser(string userMessage)
    {
        var isNotNullOrNotWhitespace = !string.IsNullOrWhiteSpace(userMessage);
        var isNotDuplicate = !_historyMessages.Any(m => m.Role == "user" && m.Content == userMessage);

        if (isNotNullOrNotWhitespace && isNotDuplicate)
        {
            //Messages.Add(new ChatMessage(ChatRole.User, userMessage));
            _historyMessages.Add(new HistoryMessage { Role = ChatRole.User.ToString(), Content = userMessage, CreatedAt = DateTimeOffset.UtcNow });
        }
    }

    public void Save()
    {
        var json = JsonSerializer.Serialize(_historyMessages, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_path, json);
    }

    public void Load()
    {
        if (File.Exists(_path))
        {
            try
            {
                var json = File.ReadAllText(_path);
                var savedList = JsonSerializer.Deserialize<List<HistoryMessage>>(json);
                _historyMessages.AddRange(savedList ?? new List<HistoryMessage>());
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read history: {ex.Message}");
            }
        }
    }

    public static List<HistoryMessage> LoadGlobalHistories(string directory = ".", string pattern = "history-*.json")
    {
        var allMessages = new List<HistoryMessage>();

        try
        {
            var files = Directory.GetFiles(directory, pattern);
            
            foreach (var file in files)
            {
                try
                {
                    var json = File.ReadAllText(file);
                    var messages = JsonSerializer.Deserialize<List<HistoryMessage>>(json);
                    if (messages != null)
                    {
                        allMessages.AddRange(messages);
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Failed to read history from {file}: {ex.Message}");
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Failed to search for history files: {ex.Message}");
        }

        return allMessages;
    }

    public List<ChatMessage> GetMessages()
    {
        return ConvertFromHistoryMessages(_historyMessages);
    }

    private List<ChatMessage> ConvertFromHistoryMessages(List<HistoryMessage> messages)
    {
        return messages.Select(ConvertFromHistoryMessage).ToList();
    }

    private ChatMessage ConvertFromHistoryMessage(HistoryMessage message)
    {
        var role = new ChatRole(new(message.Role!.ToLower()));
        return new ChatMessage(role, message.Content) { CreatedAt = message.CreatedAt };
    }


    private HistoryMessage ConvertToHistoryMessage(ChatMessage message)
    {
        return new HistoryMessage
        {
            Role = message.Role.ToString(),
            Content = message.Text,
            CreatedAt = message.CreatedAt
        };
    }

    private List<HistoryMessage> ConvertToHistoryMessages(List<ChatMessage> messages)
    {
        return messages.Select(ConvertToHistoryMessage).ToList();
    }


    // public static ChatHistory Load(string path, int items)
    // {
    //     var history = new ChatHistory();

    //     if (File.Exists(path))
    //     {
    //         try
    //         {
    //             var json = File.ReadAllText(path);
    //             var savedList = JsonSerializer.Deserialize<List<HistoryMessage>>(json);

    //             if (savedList != null)
    //             {
    //                 var selectedItems = savedList.TakeLast<HistoryMessage>(items);

    //                 foreach (var msg in selectedItems)
    //                 {
    //                     // Converte string de volta para Enum AuthorRole
    //                     if (Enum.TryParse<AuthorRole>(msg.Role, out var role))
    //                     {
    //                         history.AddMessage(role, msg.Content);
    //                     }
    //                 }
    //                 Console.WriteLine($"{savedList.Count} old messages recovered.");
    //                 return history;
    //             }
    //         }
    //         catch (Exception ex)
    //         {
    //             Console.WriteLine($"Failed to read history: {ex.Message}");
    //         }
    //     }
    //     else
    //     {
    //         history.AddMessage(AuthorRole.System, "Tu és um Assistente virtual focado em C# e .NET. Rápido e conciso. Responda sempre em Português.");
    //     }

    //         // Se não houver arquivo, inicia um novo com o Prompt do Sistema
    //         //history.AddMessage(AuthorRole.System, "Você é um assistente especialista em .NET e C#.");
    //         return history;
    // }
    public class HistoryMessage
    {
        public string? Role { get; set; } // "User", "System" ou "Assistant"
        public string? Content { get; set; }
        public DateTimeOffset? CreatedAt { get; set; } // Adicione esta propriedade para armazenar a data e hora da mensagem
    }
}