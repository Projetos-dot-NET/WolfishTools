using System.Numerics.Tensors;
using LLama;
using LLama.Common;
using Microsoft.EntityFrameworkCore;
using Wolfish.Rita; // Trazemos as Entidades do projeto ao lado

namespace Wolfish.Cadu;

// Criamos um Contexto Derivado para não Precisar reescrever o código de banco,
// apenas sobrescrevendo o arquivo final que ele enxergará
public class CaduDbContext : AppDbContext
{
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Aponta diretamente para o banco de dados populado pela Rita
        optionsBuilder.UseSqlite("Data Source=../Wolfish.Rita/app.db");
    }
}

public class Program
{
    public static async Task Main(string[] args)
    {

        Console.WriteLine("===========================================");
        Console.WriteLine(" WOLFISH.CADU - AGENTE CORPORATIVO RAG     ");
        Console.WriteLine("===========================================\n");

        // 1. Conexão com o Armazém Vetorial (Rita SQLite)
        Console.WriteLine("[1] Conectando à memória vetorial da Rita...");
        using var db = new CaduDbContext();
        
        try {
            var recordCount = db.DocumentRecords.Count();
            Console.WriteLine($"    Conexão estabelecida! Memórias disponíveis: {recordCount}");
        } catch {
            Console.WriteLine("    Aviso: Banco de dados não existe ou está vazio. É altamente recomendado popular através do Wolfish.Rita.");
        }

        // 2. Carregando Embedder (Nomic) para busca semântica
        Console.WriteLine("\n[2] Carregando motor de busca semântica (Embeddings Nomic)...");
        var embedderModelPath = "/home/renatolobojr/Downloads/nomic-embed-text-v1.5.Q8_0.gguf";
        if (!File.Exists(embedderModelPath))
        {
            Console.WriteLine($"    ERRO: Modelo Embedder não encontrado em {embedderModelPath}");
            return;
        }
        // (DICA FUTURA: Adicione ', GpuLayerCount = 99' no fim se for usar o Backend Cuda12 da NVIDIA depois)
        var embedderParams = new ModelParams(embedderModelPath) { ContextSize = 1024, Embeddings = true };
        using var embedderWeights = LLamaWeights.LoadFromFile(embedderParams);
        using var embedder = new LLamaEmbedder(embedderWeights, embedderParams);

        // 3. Carregando Cérebro Agente (Qwen2.5) para leitura e escrita real
        Console.WriteLine("\n[3] Carregando o Cérebro LLM do Cadu (Qwen2.5/Llama)...");
        var llmModelPath = "/home/renatolobojr/Downloads/qwen2.5-1.5b-instruct-q8_0.gguf"; 
        
        if (!File.Exists(llmModelPath))
        {
            Console.WriteLine($"    ERRO: Modelo LLM não encontrado em {llmModelPath}");
            return;
        }
        var llmParams = new ModelParams(llmModelPath)
        {
            ContextSize = 4096, // O LLM gerativo vai precisar de mais contexto nativo para ler os "Chunks" recuperados da Rita
            
            // ============== CONFIGURAÇÃO FUTURA (GPU NVIDIA RTX 2060 SUPER) ==============
            // Quando quiser migrar os cálculos para sua placa de vídeo:
            // 1. Exclua o backend de CPU: dotnet remove package LLamaSharp.Backend.Cpu
            // 2. Instale o backend Cuda:  dotnet add package LLamaSharp.Backend.Cuda12
            // 3. Modifique esse número de '20' para '99' ou '-1' (Carregamento Total)
            GpuLayerCount = 20  
        };
        using var llmWeights = LLamaWeights.LoadFromFile(llmParams);
        
        // Executor Stateless processa Prompt + Contexto numa cajadada só, sem precisar de histórico prolongado (ideal para RAG simples)
        // Obs: Em versões mais novas usa (weights, params), ou simplesmente usamos InstructExecutor se Stateless não suportar a arq.
        var executor = new StatelessExecutor(llmWeights, llmParams); 

        Console.WriteLine("\n🎉 Motores de IA inicializados e Conectados com sucesso!\n");

        // ======= LOOP DE CHAT =======
        while (true)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.Write("\nVocê: ");
            Console.ResetColor();
            var userInput = Console.ReadLine();

            if (string.IsNullOrWhiteSpace(userInput)) continue;
            if (userInput.Equals("sair", StringComparison.OrdinalIgnoreCase)) break;

            // PASSO A: Transformar a Pergunta do Usuário em Vetor e Buscar na Rita
            var queryEmbeddingsResult = await embedder.GetEmbeddings(userInput);
            float[] queryEmbedding = queryEmbeddingsResult.Count > 0 ? queryEmbeddingsResult[0] : Array.Empty<float>();

            var allRecords = db.DocumentRecords.ToList();
            var results = new List<(float Score, DocumentRecord Record)>();

            foreach (var record in allRecords)
            {
                if (record.Embedding == null || record.Embedding.Length == 0) continue;
                var score = TensorPrimitives.CosineSimilarity(queryEmbedding, record.Embedding);
                results.Add((score, record));
            }

            // Puxa os Top 3 similares
            var top3 = results.OrderByDescending(x => x.Score).Take(3).ToList();
            
            // PASSO B: Montar a Cadeia de Contexto em Texto Bruto
            string recoveredContext = "";
            if (top3.Any())
            {
                // A cada citação do RAG, nós computamos a validade daquele registro e o persistimos em definitivo para não ser deletado pelo Garbage Collector da Rita
                foreach(var item in top3) 
                {
                    item.Record.RetrievalCount++;
                }
                db.SaveChanges();

                int rank = 1;
                foreach(var item in top3)
                {
                    recoveredContext += $"[Trecho Recuperado {rank} (Ref Similaridade: {item.Score:F2})]: {item.Record.TextContent}\n";
                    rank++;
                }
            }
            else
            {
                recoveredContext = "Nenhum contexto histórico adicional encontrado na base de dados.";
            }

            // PASSO C: Montar o Prompt Injetado (Prompt Engineering) -> Formato genérico Qwen/ChatML
            string prompt = $@"<|im_start|>system
Você é o Cadu, um experiente e educado assistente virtual da empresa.
Sua missão é ler o contexto recuperado do banco de dados e conversar com o usuário entregando a resposta elaborada em formato de diálogo amigável.
Baseie sua resposta EXCLUSIVAMENTE nos trechos abaixo. Se a informação não estiver no contexto, diga gentilmente que não encontrou essa informação nos registros da Rita. Nunca invente dados.

INFORMAÇÕES DE CONTEXTO DA EMPRESA:
{recoveredContext}
<|im_end|>
<|im_start|>user
{userInput}
<|im_end|>
<|im_start|>assistant
";

            // PASSO D: Customizar a Criatividade da Máquina
            // (Nota: Temperature e TopP foram movidos para IGSamplingPipeline em versões mais recentes do LLamaSharp, usamos as propriedades raízes)
            var inferenceParams = new InferenceParams()
            {
                MaxTokens = 512,
                AntiPrompts = ["<|im_end|>", "<|im_start|>"] // Impede que o bot alucine tentando falar pelos dois lados
            };

            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.Write("\nCadu: ");
            Console.ResetColor();

            // PASSO E: Executar LLM em Streaming
            await foreach (var text in executor.InferAsync(prompt, inferenceParams))
            {
                Console.Write(text);
            }
            Console.WriteLine();
        }
    }
}
