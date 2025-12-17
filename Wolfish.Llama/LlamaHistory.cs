using LLama.Common;
using System.Text.Json;

public static class LlamaHistory
{
    private static readonly string file = "chat_history.json";

    public static void Save(ChatHistory history)
    {
        // Converte o histórico do LLamaSharp para nossa lista simples
        var listToSave = history.Messages.Select(m => new HistoryMessage
        {
            Role = m.AuthorRole.ToString(),
            Content = m.Content
        }).ToList();

        var json = JsonSerializer.Serialize(listToSave, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(file, json);
        Console.WriteLine($"Chat save in {file}");
    }

    public static ChatHistory Load()
    {
        var history = new ChatHistory();

        if (File.Exists(file))
        {
            try
            {
                var json = File.ReadAllText(file);
                var savedList = JsonSerializer.Deserialize<List<HistoryMessage>>(json);

                if (savedList != null)
                {
                    foreach (var msg in savedList)
                    {
                        // Converte string de volta para Enum AuthorRole
                        if (Enum.TryParse<AuthorRole>(msg.Role, out var role))
                        {
                            history.AddMessage(role, msg.Content);
                        }
                    }
                    Console.WriteLine($"{savedList.Count} old messages recovered.");
                    return history;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to read history: {ex.Message}");
            }
        }

        // Se não houver arquivo, inicia um novo com o Prompt do Sistema
        //history.AddMessage(AuthorRole.System, "Você é um assistente especialista em .NET e C#.");
        return history;
    }
    public class HistoryMessage
    {
        public string Role { get; set; } // "User", "System" ou "Assistant"
        public string Content { get; set; }
    }
}