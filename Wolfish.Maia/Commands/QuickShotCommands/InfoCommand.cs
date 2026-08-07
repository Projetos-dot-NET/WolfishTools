using System.Reflection;
using System.Runtime.InteropServices;

namespace Wolfish.Maia.Commands
{
    public class InfoCommand : ICliCommand
    {
        public string Name => "info";

        public Task ExecuteAsync(string[] args)
        {
            var basedir = AppContext.BaseDirectory;
            var semver = Assembly.GetEntryAssembly()?.GetCustomAttribute<AssemblyInformationalVersionAttribute>()?.InformationalVersion;

            var infoSO = RuntimeInformation.OSDescription;
            var arch = RuntimeInformation.OSArchitecture.ToString();
            var runtime = RuntimeInformation.RuntimeIdentifier;
            var platform = RuntimeInformation.IsOSPlatform(OSPlatform.Windows) ? "Windows" :
                           RuntimeInformation.IsOSPlatform(OSPlatform.Linux) ? "Linux" :
                           RuntimeInformation.IsOSPlatform(OSPlatform.OSX) ? "macOS" : "Unknown OS";

            Console.WriteLine("Software: Wolfish.Maia");
            Console.WriteLine("Author: Renato Lobo Jr.");
            Console.WriteLine("Licence: MIT");            
            Console.WriteLine("Version: {0}", semver);
            Console.WriteLine("GitHub: https://github.com/renatolobojr/WolfishTools");            
            Console.WriteLine("OS: {0} {1} ({2} based in {3})", platform, arch, infoSO, runtime);            
            Console.WriteLine("Base Directory: {0}", basedir);
            
            return Task.CompletedTask;
        }
    }
}
