using System.Text.Json;
using System.Text.Json.Nodes;
using System.ComponentModel;
using Microsoft.Extensions.Logging;
using ModelContextProtocol.Server;

internal class ConfigTools(ILogger<ConfigTools> logger)
{
    private const string FilePath = "teste.json";

    [McpServerTool]
    [Description("Lê ou atualiza um parâmetro específico em um arquivo JSON.")]
    public string EditarConfig(
        [Description("A chave/parâmetro que deseja alterar")] string chave,
        [Description("O novo valor para este parâmetro")] string valor)
    {
        try
        {
            JsonNode? root;
            
            // 1. Ler o arquivo existente ou criar um novo objeto
            if (File.Exists(FilePath))
            {
                string jsonString = File.ReadAllText(FilePath);
                root = JsonNode.Parse(jsonString) ?? new JsonObject();
            }
            else
            {
                root = new JsonObject();
            }

            // 2. Atualizar o valor (salvamos tudo como string para simplificar a PoC)
            root[chave] = valor;

            // 3. Salvar de volta no disco
            var options = new JsonSerializerOptions { WriteIndented = true };
            File.WriteAllText(FilePath, root.ToJsonString(options));

            return $"Sucesso: Parâmetro '{chave}' atualizado para '{valor}' no arquivo {FilePath}.";
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Erro ao editar arquivo JSON");
            return $"Erro: {ex.Message}";
        }
    }
}
