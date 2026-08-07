using System.Reflection;

namespace Wolfish.Maia.Commands
{
    public class WelcomeCommand : ICliCommand
    {
        public string Name => "welcome";

        public Task ExecuteAsync(string[] args)
        {
            Console.WriteLine($"\nThank you! I'm happy to be here!");
            return Task.CompletedTask;
        }
    }
}
