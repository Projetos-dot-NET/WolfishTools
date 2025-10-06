namespace Wolfish.Maia
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.Write("Hello, World!");
            foreach (var arg in args)
            {
                Console.Write($" {arg}");
            }
        }
    }
}
