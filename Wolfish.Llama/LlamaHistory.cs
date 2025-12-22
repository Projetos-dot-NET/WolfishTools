using LLama.Common;
using System.Text.Json;

public static class LlamaHistory
{
    public static void Save(ChatHistory history, string path)
    {
        // Converte o histórico do LLamaSharp para nossa lista simples
        var listToSave = history.Messages.Select(m => new HistoryMessage
        {
            Role = m.AuthorRole.ToString(),
            Content = m.Content
        }).ToList();

        var json = JsonSerializer.Serialize(listToSave, new JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(path, json);
    }

    public static ChatHistory Load(string path, int items)
    {
        var history = new ChatHistory();

        if (File.Exists(path))
        {
            try
            {
                var json = File.ReadAllText(path);
                var savedList = JsonSerializer.Deserialize<List<HistoryMessage>>(json);

                if (savedList != null)
                {
                    var selectedItems = savedList.TakeLast<HistoryMessage>(items);

                    foreach (var msg in selectedItems)
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
        else
        {
            history.AddMessage(AuthorRole.System, "Tu és um Assistente virtual focado em C# e .NET. Rápido e conciso. Responda sempre em Português.");
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