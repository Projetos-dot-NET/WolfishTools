namespace Wolfish.Maia.Commands
{
    /// <summary>
    /// Interface para comandos CLI de argumento único (quick shots).
    /// Para adicionar um novo comando, crie uma classe que implemente esta interface
    /// e registre-a no CommandRegistry.
    /// </summary>
    public interface ICliCommand
    {
        /// <summary>
        /// Nome do comando (ex: "welcome", "list", "platform").
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Executa o comando.
        /// </summary>
        Task ExecuteAsync(string[] args);
    }
}
