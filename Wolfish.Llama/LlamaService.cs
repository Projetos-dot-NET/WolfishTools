using LLama;
using LLama.Common;
using LLama.Native;

namespace Wolfish.Llama
{
    public class LlamaService
    {
        //private readonly string _path;
        private readonly LlamaSettings _settings;

        public LlamaService(LlamaSettings settings)
        {
            _settings = settings;
        }

        public async Task ChatWithAgent(string answer)
        {
            NativeLogConfig.llama_log_set((level, message) => { }); //silenciar os logs nativos do llama

            var parameters = new ModelParams(_settings.ModelPath) 
            {
                ContextSize = _settings.ContextSize, 
                GpuLayerCount = _settings.GpuLayerCount,
                Threads = _settings.Threads,
                BatchThreads = _settings.Threads
            };


            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);
            var executor = new InteractiveExecutor(context);

            ChatHistory chatHistory = LlamaHistory.Load();
            chatHistory.AddMessage(AuthorRole.System, _settings.SystemMessage);

            var session = new ChatSession(executor, chatHistory);
            //var responseBuffer = "";

            await foreach (var text in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, answer), new InferenceParams { AntiPrompts = _settings.AntiPrompts }))
            {
                Console.Write(text);

                //caso o chat fique preso na resposta, descomente as linhas abaixo para forçar a parada                
                //if (text.Contains("<end_of_turn>") || text.Contains("<eos>") || text.Contains("User:")) break;                

                //tentativa mais rigorosa de parada
                //responseBuffer += text;
                //if (responseBuffer.EndsWith("\r\n\r\n\r\n\r\n\r\n\r\n")) break;
            }

            LlamaHistory.Save(chatHistory);

        }

        /* EXEMPLOS DE TESTES ANTERIORES COM A GEMMA 
        public async Task ChatWithGemma6(string answer, string instruction)
        {
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 2048,
                GpuLayerCount = 0
            };

            Console.WriteLine("Carregando Gemma...");
            using var model = LLamaWeights.LoadFromFile(parameters);
            using var context = model.CreateContext(parameters);

            // Usamos o executor interativo, mas SEM a classe 'ChatSession'
            var executor = new InteractiveExecutor(context);

            // 2. CONFIGURAÇÃO DE INFERÊNCIA
            var inferenceParams = new InferenceParams()
            {
                MaxTokens = -1, 
                AntiPrompts = new List<string> { "<start_of_turn>" }
            };

            Console.WriteLine(">>> PRONTO (MODO RAW/CRU) <<<");

            while (true)
            {
                Console.Write("\nVocê: ");
                var input = Console.ReadLine();
                if (string.IsNullOrWhiteSpace(input)) break;

                // --- O SEGREDO ESTÁ AQUI ---
                // Estamos construindo o prompt manualmente no formato que a Gemma exige.
                // Sem isso, ela fica muda.
                var promptFormatado = $"<start_of_turn>user\n{input}<end_of_turn>\n<start_of_turn>model\n";

                Console.Write("Gemma: ");

                // Chamamos o InferAsync direto, passando a string formatada
                await foreach (var text in executor.InferAsync(promptFormatado, inferenceParams))
                {
                    Console.Write(text);
                }
            }
        }

        public async Task ChatWithGemma5(string answer, string instruction)
        {
            Console.WriteLine("Teste de vida do modelo...");

            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024,
                GpuLayerCount = 0
            };

            Console.WriteLine("2. Carregando pesos do modelo...");
            using var model = LLamaWeights.LoadFromFile(parameters);

            Console.WriteLine("3. Criando contexto...");
            using var context = model.CreateContext(parameters);

            var executor = new InteractiveExecutor(context);
            var session = new ChatSession(executor);

            var prompt = "Explain quantum physics in simple terms."; // Inglês costuma funcionar melhor se o prompt template estiver quebrado
            var inferenceParams = new InferenceParams() { MaxTokens = 200};

            // Vamos usar o executor direto, sem ChatSession para isolar o erro
            await foreach (var text in executor.InferAsync(prompt, inferenceParams))
            {
                Console.Write(text);
            }
        }

        public async Task ChatWithGemma4(string answer, string instruction)
        {
            Console.WriteLine("1. Inicializando parâmetros...");
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 1024,
                GpuLayerCount = 0
            };

            try
            {
                Console.WriteLine("2. Carregando pesos do modelo...");
                using var model = LLamaWeights.LoadFromFile(parameters);

                Console.WriteLine("3. Criando contexto...");
                using var context = model.CreateContext(parameters);

                var executor = new InteractiveExecutor(context);
                var session = new ChatSession(executor);

                Console.WriteLine("\n>>> TUDO PRONTO. TESTE RÁPIDO <<<");
                Console.WriteLine("Pergunta: 'Quem é você?'");

                // --- TESTE SIMPLIFICADO ---
                // Aqui mudamos a estratégia. Vamos coletar a resposta numa lista
                // para ter certeza que não é erro de impressão no Console.

                var prompt = "Quem é você? Responda em uma frase.";
                var inferenceParams = new InferenceParams() { MaxTokens = 1024, AntiPrompts = new List<string> { "User:", "Você:" } };

                Console.Write("Resposta da Gemma: ");

                // Tente esta sintaxe exata
                await foreach (string token in session.ChatAsync(new ChatHistory.Message(AuthorRole.User, prompt), inferenceParams))
                {
                    // Se cair aqui, o modelo está gerando algo.
                    Console.Write(token);
                }

                Console.WriteLine("\n\nTeste finalizado.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"\nERRO DETALHADO: {ex.GetType().Name}");
                Console.WriteLine(ex.Message);
                Console.WriteLine(ex.StackTrace);
            }

        }


        public async Task ChatWithGemma3(string answer, string instruction)
        {
            // --- CONFIGURAÇÃO ---
            // COLOQUE O CAMINHO DO SEU ARQUIVO AQUI:
            //string modelPath = "/home/seu_usuario/Downloads/gemma-2-2b-it.Q4_K_M.gguf";

            // Configurações de carregamento do modelo
            var parameters = new ModelParams(modelPath)
            {
                ContextSize = 2048, // Tamanho da "memória" da conversa
                GpuLayerCount = 0   // Deixe 0 se usar CPU. Se usar GPU, coloque 20 ou mais.
            };

            Console.WriteLine("Carregando o modelo Gemma... (isso pode levar alguns segundos)");

            try
            {
                using var model = LLamaWeights.LoadFromFile(parameters);
                using var context = model.CreateContext(parameters);
                var executor = new InteractiveExecutor(context);

                // Cria o histórico do chat
                var chatHistory = new ChatHistory();
                chatHistory.AddMessage(AuthorRole.System, "Você é um assistente útil e inteligente.");

                var session = new ChatSession(executor, chatHistory);

                // Configurações de inferência (temperatura, etc)
                var inferenceParams = new InferenceParams()
                {
                    AntiPrompts = new List<string> { "User:" }
                };

                Console.Clear();
                Console.WriteLine(">>> Modelo Gemma Carregado! <<<");
                Console.WriteLine("Digite 'sair' para encerrar.\n");

                // Loop de conversa
                while (true)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.Write("Você: ");
                    Console.ResetColor();

                    var input = Console.ReadLine();
                    if (string.IsNullOrWhiteSpace(input) || input.ToLower() == "sair") break;

                    Console.ForegroundColor = ConsoleColor.Cyan;
                    Console.Write("Gemma: ");

                    // Gera a resposta em tempo real (streaming)
                    var responseAsync = session.ChatAsync(new ChatHistory.Message(AuthorRole.User, input), inferenceParams);

                    await foreach (var text in responseAsync)
                    {
                        Console.Write(text);
                    }

                    Console.WriteLine(); // Quebra de linha ao final da resposta
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\nERRO: Não foi possível carregar o modelo.\nVerifique se o caminho do arquivo está correto: {modelPath}");
                Console.WriteLine($"Detalhe do erro: {ex.Message}");
                Console.ResetColor();
            }
        }*/

    }
}
