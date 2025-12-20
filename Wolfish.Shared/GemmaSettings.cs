public class GemmaSettings
{
    public string ModelPath { get; set; } = "";
    public int ContextSize { get; set; } = 2048;
    public int GpuLayerCount { get; set; } = 0;
    public string HistoryFileName { get; set; } = "chat_history.json";
    public int MaxTokens { get; set; } = 1024;
    public float Temperature { get; set; } = 0.7f;
}