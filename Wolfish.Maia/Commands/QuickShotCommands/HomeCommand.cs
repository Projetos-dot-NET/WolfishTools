using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.Extensions.Configuration;

namespace Wolfish.Maia.Commands
{
    /// <summary>
    /// Comando "home" — define a pasta MyHome no appsettings.json.
    /// Uso: home [caminho]
    ///   - Sem argumento: usa o diretório atual do CLI
    ///   - Com argumento: usa o caminho informado
    /// Se MyHome já estiver definido, pede confirmação antes de substituir.
    /// </summary>
    public class HomeCommand : ICliCommand
    {
        public string Name => "home";

        public Task ExecuteAsync(string[] args)
        {
            var baseDirectory = AppContext.BaseDirectory;
            var appsettingsPath = Path.Combine(baseDirectory, "appsettings.json");

            // Lê o valor atual de MyHome
            var config = new ConfigurationBuilder()
                .SetBasePath(baseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                .Build();

            var currentHome = config["MyHome"];

            // Define o novo caminho: argumento informado ou diretório atual
            var newHome = args.Length >= 2 && !string.IsNullOrWhiteSpace(args[1])
                ? Path.GetFullPath(args[1])
                : Directory.GetCurrentDirectory();

            Console.WriteLine();

            // Se já existe um MyHome definido, pede confirmação
            if (!string.IsNullOrWhiteSpace(currentHome))
            {
                Console.WriteLine($"  MyHome atual: {currentHome}");
                Console.WriteLine($"  Novo caminho: {newHome}");
                Console.WriteLine();

                // Se o caminho é o mesmo, não precisa alterar
                if (string.Equals(Path.GetFullPath(currentHome), Path.GetFullPath(newHome), StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("  ℹ️  O caminho já é o mesmo. Nada a alterar.\n");
                    return Task.CompletedTask;
                }

                Console.Write("  Deseja substituir? (s/N): ");
                var resposta = Console.ReadLine()?.Trim();

                if (!string.Equals(resposta, "s", StringComparison.OrdinalIgnoreCase) &&
                    !string.Equals(resposta, "sim", StringComparison.OrdinalIgnoreCase))
                {
                    Console.WriteLine("\n  Operação cancelada.\n");
                    return Task.CompletedTask;
                }
            }

            // Atualiza o appsettings.json
            try
            {
                var json = File.ReadAllText(appsettingsPath);
                var jsonNode = JsonNode.Parse(json, documentOptions: new JsonDocumentOptions
                {
                    CommentHandling = JsonCommentHandling.Skip,
                    AllowTrailingCommas = true
                });

                if (jsonNode is JsonObject root)
                {
                    root["MyHome"] = newHome;

                    var writeOptions = new JsonSerializerOptions
                    {
                        WriteIndented = true,
                        Encoder = System.Text.Encodings.Web.JavaScriptEncoder.UnsafeRelaxedJsonEscaping
                    };

                    File.WriteAllText(appsettingsPath, jsonNode.ToJsonString(writeOptions));

                    Console.WriteLine($"\n  ✅ MyHome definido: {newHome}\n");
                }
                else
                {
                    Console.WriteLine("\n  ❌ Erro: appsettings.json não é um objeto JSON válido.\n");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\n  ❌ Erro ao salvar appsettings.json: {ex.Message}\n");
            }

            return Task.CompletedTask;
        }
    }
}
