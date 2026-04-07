using System.Numerics.Tensors;
using LLama;
using LLama.Common;
using Microsoft.EntityFrameworkCore;
using Wolfish.Rita;

Console.WriteLine("Iniciando a aplicação de Embeddings com SQLite e LLamaSharp...");

// Configuração do Banco de Dados
using var db = new AppDbContext();
Console.WriteLine("Garantindo a criação do banco de dados...");
db.Database.EnsureCreated();

// Garante que a coluna RetrievalCount exista em bancos antigos (Evita quebrar o SQLite previamente construído)
try
{
    db.Database.ExecuteSqlRaw("ALTER TABLE DocumentRecords ADD COLUMN RetrievalCount INTEGER NOT NULL DEFAULT 0;");
}
catch { /* Tolerado caso a coluna já exista no schema atual */ }

// Solicita o caminho do modelo
Console.WriteLine("Carregando o modelo apontado no código...");
var modelPath = "/home/renatolobojr/Downloads/nomic-embed-text-v1.5.Q8_0.gguf";

if (string.IsNullOrWhiteSpace(modelPath) || !File.Exists(modelPath))
{
    Console.WriteLine("Caminho do modelo inválido ou arquivo não encontrado. Encerrando.");
    return;
}

// Configuração do LLamaSharp para Embeddings
var parameters = new ModelParams(modelPath)
{
    ContextSize = 1024,
    Embeddings = true // CRÍTICO: Ativa a geração de embeddings!
};

using var model = LLamaWeights.LoadFromFile(parameters);
using var embedder = new LLamaEmbedder(model, parameters);

Console.WriteLine("Modelo carregado com sucesso!");

// Exemplo de Inserção de Dados (Seed inicial)
var documentTexts = new[]
{
    "A inteligência artificial ajuda a automatizar tarefas diárias.",
    "Bancos de dados relacionais organizam informações em tabelas.",
    "O Entity Framework Core é um framework de mapeamento objeto-relacional.",
    "A receita de pão de queijo leva polvilho doce e queijo minas.",
    "Carros elétricos não emitem gases poluentes pelo escapamento."
};

Console.WriteLine("Verificando banco de dados...");
if (!db.DocumentRecords.Any())
{
    Console.WriteLine("Gerando embeddings e inserindo novos documentos de teste no SQLite...");
    foreach (var text in documentTexts)
    {
        var embeddingsResult = await embedder.GetEmbeddings(text);
        float[] embedding = embeddingsResult.Count > 0 ? embeddingsResult[0] : Array.Empty<float>();

        var record = new DocumentRecord { TextContent = text, Embedding = embedding };
        db.DocumentRecords.Add(record);
    }
    db.SaveChanges();
    Console.WriteLine("Documentos iniciais inseridos com sucesso!");
}

// LOOP PRINCIPAL DE CRUD
while (true)
{
    Console.WriteLine("\n===========================================");
    Console.WriteLine(" MENU - GERENCIADOR DE EMBEDDINGS (CRUD)");
    Console.WriteLine("===========================================");
    Console.WriteLine("1. Buscar texto por similaridade");
    Console.WriteLine("2. Adicionar novo texto");
    Console.WriteLine("3. Listar todos os textos registrados");
    Console.WriteLine("4. Editar um texto");
    Console.WriteLine("5. Excluir um texto");
    Console.WriteLine("6. Importar texto a partir de arquivo (.txt/.md)");
    Console.WriteLine("7. [ADMIN] Limpar memórias inativas do Cadu (Esquecimento)");
    Console.WriteLine("0. Sair");
    Console.Write("\nEscolha uma opção: ");

    var option = Console.ReadLine();
    if (option == "0") break;

    switch (option)
    {
        case "1": // BUSCAR
            Console.Write("\nDigite a frase para buscar por similaridade: ");
            var query = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(query)) break;

            var queryEmbeddingsResult = await embedder.GetEmbeddings(query);
            float[] queryEmbedding = queryEmbeddingsResult.Count > 0 ? queryEmbeddingsResult[0] : Array.Empty<float>();

            var allRecords = db.DocumentRecords.ToList();
            var results = new List<(float Score, DocumentRecord Record)>();

            foreach (var record in allRecords)
            {
                if (record.Embedding == null || record.Embedding.Length == 0) continue;
                var score = TensorPrimitives.CosineSimilarity(queryEmbedding, record.Embedding);
                results.Add((score, record));
            }

            var top3 = results.OrderByDescending(x => x.Score).Take(3).ToList();

            if (top3.Any())
            {
                Console.WriteLine($"\n>>> [TOP {top3.Count} RESULTADOS ENCONTRADOS] <<<");
                int rank = 1;
                foreach (var item in top3)
                {
                    item.Record.RetrievalCount++; // +1 na Estatística
                    db.SaveChanges(); // Persiste

                    Console.WriteLine($"\n#{rank} | ID: {item.Record.Id} | Acessos Históricos: {item.Record.RetrievalCount} | Score de Similaridade: {item.Score:F4}");
                    
                    var preview = item.Record.TextContent.Length > 250 
                        ? item.Record.TextContent.Substring(0, 250) + " [...]" 
                        : item.Record.TextContent;
                        
                    Console.WriteLine($"Texto Original: {preview}");
                    rank++;
                }
            }
            else
            {
                Console.WriteLine("\nNenhum resultado válido encontrado.");
            }
            break;

        case "2": // ADICIONAR
            Console.Write("\nDigite o novo texto que deseja adicionar: ");
            var newText = Console.ReadLine();
            if (string.IsNullOrWhiteSpace(newText)) break;

            Console.WriteLine("Gerando Embedding...");
            var newEmbeddingsResult = await embedder.GetEmbeddings(newText);
            float[] newEmbedding = newEmbeddingsResult.Count > 0 ? newEmbeddingsResult[0] : Array.Empty<float>();

            db.DocumentRecords.Add(new DocumentRecord { TextContent = newText, Embedding = newEmbedding });
            db.SaveChanges();
            Console.WriteLine(">>> Texto e vector embedding salvos no SQLite com sucesso!");
            break;

        case "3": // LISTAR
            Console.WriteLine("\n>>> [TODOS OS REGISTROS DO BANCO]");
            var list = db.DocumentRecords.ToList();
            if (list.Count == 0) Console.WriteLine("O banco de dados está vazio.");
            foreach (var record in list)
            {
                Console.WriteLine($"[ID: {record.Id}] [Usos: {record.RetrievalCount}] - {record.TextContent.Substring(0, 50)}...");
            }
            break;

        case "4": // EDITAR
            Console.Write("\nDigite o ID do texto que deseja EDITAR (use a opção 3 para ver os IDs): ");
            if (int.TryParse(Console.ReadLine(), out int editId))
            {
                var recordToEdit = db.DocumentRecords.Find(editId);
                if (recordToEdit != null)
                {
                    Console.WriteLine($"Texto atual: {recordToEdit.TextContent}");
                    Console.Write("Digite o NOVO texto (isso irá re-calcular o embedding no backend): ");
                    var updatedText = Console.ReadLine();

                    if (!string.IsNullOrWhiteSpace(updatedText))
                    {
                        Console.WriteLine("Gerando novo Embedding...");
                        var editedEmbeddingsResult = await embedder.GetEmbeddings(updatedText);

                        recordToEdit.TextContent = updatedText;
                        recordToEdit.Embedding = editedEmbeddingsResult.Count > 0 ? editedEmbeddingsResult[0] : Array.Empty<float>();
                        db.SaveChanges();
                        Console.WriteLine(">>> Texto atualizado no banco SQLite com sucesso!");
                    }
                }
                else Console.WriteLine(">>> Erro: ID não encontrado no banco de dados.");
            }
            break;

        case "5": // DELETAR
            Console.Write("\nDigite o ID do texto que deseja EXCLUIR: ");
            if (int.TryParse(Console.ReadLine(), out int deleteId))
            {
                var recordToDelete = db.DocumentRecords.Find(deleteId);
                if (recordToDelete != null)
                {
                    db.DocumentRecords.Remove(recordToDelete);
                    db.SaveChanges();
                    Console.WriteLine(">>> Registro deletado permanentemente do banco SQLite!");
                }
                else Console.WriteLine(">>> Erro: ID não encontrado no banco de dados.");
            }
            break;

        case "6": // IMPORTAR ARQUIVO
            Console.Write("\nDigite o caminho completo (absoluto) para o arquivo .txt ou .md: ");
            var filePath = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(filePath) || !File.Exists(filePath))
            {
                Console.WriteLine($">>> Erro: Arquivo inválido ou não encontrado. [{filePath}]");
                break;
            }

            try
            {
                var fileContent = await File.ReadAllTextAsync(filePath);
                if (string.IsNullOrWhiteSpace(fileContent))
                {
                    Console.WriteLine(">>> Erro: O arquivo está vazio.");
                    break;
                }

                Console.WriteLine($"Lendo {fileContent.Length} caracteres. Gerando Embeddings da Rede Neural usando Algoritmo de Chunking...");
                
                var fileName = Path.GetFileName(filePath);
                
                // Variáveis configuráveis de tamanho (Chunking e Overlap)
                int maxChunkSize = 700; // Tamanho máximo de recorte
                int overlapSize = 100;  // Intersecção em caracteres que deve cruzar entre as fatias
                
                int chunkCounter = 0;
                int currentIndex = 0;

                while (currentIndex < fileContent.Length)
                {
                    int length = Math.Min(maxChunkSize, fileContent.Length - currentIndex);
                    
                    // Tenta não cortar palavra e sim quebrar buscando o último espaço daquele bloco
                    if (currentIndex + length < fileContent.Length)
                    {
                        int lastSpace = fileContent.LastIndexOf(' ', currentIndex + length - 1, length);
                        if (lastSpace > currentIndex)
                        {
                            length = lastSpace - currentIndex;
                        }
                    }

                    string chunkText = fileContent.Substring(currentIndex, length).Trim();
                    
                    var textToSave = $"[Arquivo: {fileName} | Parte {++chunkCounter}]\n{chunkText}";

                    var fileEmbeddingsResult = await embedder.GetEmbeddings(textToSave);
                    float[] fileEmbedding = fileEmbeddingsResult.Count > 0 ? fileEmbeddingsResult[0] : Array.Empty<float>();

                    db.DocumentRecords.Add(new DocumentRecord { TextContent = textToSave, Embedding = fileEmbedding });

                    // Avança o cursor iterativo, mas garante que voltamos o tamanho do "overlap" e cortamos nos espaços a seguir.
                    currentIndex += length;
                    
                    if (currentIndex < fileContent.Length)
                    {
                        currentIndex -= overlapSize;
                        if (currentIndex < 0) currentIndex = 0;
                        
                        // Otimização: garante que o próximo chunk inicie encostado numa palavra inteira
                        if (currentIndex > 0)
                        {
                            int spaceIndex = fileContent.IndexOf(' ', currentIndex);
                            if (spaceIndex != -1 && spaceIndex < fileContent.Length)
                            {
                                currentIndex = spaceIndex + 1;
                            }
                        }
                    }
                }

                db.SaveChanges();
                Console.WriteLine($">>> Sucesso! O documento foi fatiado em {chunkCounter} blocos de tamanho reduzido e importado para o Banco Vetorial!");
            }
            catch (Exception ex)
            {
                Console.WriteLine($">>> Erro interno ao processar o arquivo: {ex.Message}");
            }
            break;

        case "7": // LIMPEZA LIXO
            // Seleciona os 10 vetores com a menor pontuação histórica de utilização
            var lowestScoringRecords = db.DocumentRecords.OrderBy(r => r.RetrievalCount).Take(10).ToList();
            
            if (lowestScoringRecords.Count == 0) 
            {
                Console.WriteLine("\n>>> O banco de dados está completamente vazio.");
            }
            else 
            {
                Console.WriteLine("\n>>> [TOP 10 PIORES MEMÓRIAS (Menos Acessadas)]");
                foreach (var rec in lowestScoringRecords)
                {
                    Console.WriteLine($"[ID: {rec.Id}] Acessos: {rec.RetrievalCount} | Texto: {rec.TextContent.Substring(0, Math.Min(30, rec.TextContent.Length))}...");
                }

                Console.Write($"\nDeseja realizar ativamente o ESQUECIMENTO das {lowestScoringRecords.Count} memórias listadas acima para liberar os discos vetoriais? (s/n): ");
                if (Console.ReadLine()?.ToLower().Trim() == "s")
                {
                    db.DocumentRecords.RemoveRange(lowestScoringRecords);
                    db.SaveChanges();
                    Console.WriteLine($">>> Poda efetuada com louvor! O Agente esqueceu permanentemente os vetores obsoletos.");
                }
            }
            break;

        default:
            Console.WriteLine("\nOpção inválida. Tente novamente.");
            break;
    }
}
