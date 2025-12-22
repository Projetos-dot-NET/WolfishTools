public class LlamaSettings
{
    public string Name { get; set; }
    public string ModelPath { get; set; } = "";
    public string SystemMessage { get; set; } = "";
    public string[] AntiPrompts { get; set; } = ["User:"];
    public uint ContextSize { get; set; } = 2048;
    public int GpuLayerCount { get; set; } = 0; //0 para o cpu
    public int Threads { get; set; } = 2; //1 para o gpu
    public string HistoryFileName { get; set; } = "chat_history.json";
    public int MaxTokens { get; set; } = 1024;
    
    //public float Temperature { get; set; } = 0.7f;
}