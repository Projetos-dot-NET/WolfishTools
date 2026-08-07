namespace Wolfish.Maia.Commands
{
    public class HelpCommand : ICliCommand
    {
        public string Name => "help";

        public Task ExecuteAsync(string[] args)
        {
            Console.WriteLine("Wolfish.Maia - Assistente de linha de comando impulsionado por IA");
            Console.WriteLine("Uso:");
            Console.WriteLine("  maia welcome                     Exibe uma mensagem de boas-vindas.");
            Console.WriteLine("  maia list                        Lista todos os comandos disponíveis.");
            Console.WriteLine("  maia platform                    Exibe info do sistema operacional.");
            Console.WriteLine("  maia directory                   Exibe o diretório base do aplicativo.");
            Console.WriteLine("  maia help                        Exibe esta mensagem de ajuda.");
            Console.WriteLine("  maia install <nome_do_pacote>    Instala o pacote especificado.");
            Console.WriteLine("  maia uninstall <nome_do_pacote>  Desinstala o pacote especificado.");
            Console.WriteLine("  maia ask <pergunta>              Faz uma pergunta ao assistente de IA.");
            Console.WriteLine();
            return Task.CompletedTask;
        }
    }
}
