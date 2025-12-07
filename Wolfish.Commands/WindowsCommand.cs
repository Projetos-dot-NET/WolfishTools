//using System.Text.Json;
//using Wolfish.Commands;

//var path = "./WindowsCommands.json";

//using var fileStream = File.OpenRead(path);

//List<WolfishCommand>? pessoas = await JsonSerializer.DeserializeAsync<List<WolfishCommand>>(fileStream);

//foreach (var p in pessoas)
//{
//    Console.WriteLine($"{p.Id} - {p.Nome}");
//}



//var caminho = "pessoas.json";

//string json = File.ReadAllText(caminho);

//List<WolfishCommand>? pessoas = JsonSerializer.Deserialize<List<WolfishCommand>>(json);

//foreach (var pessoa in pessoas)
//{
//    Console.WriteLine($"{pessoa.Id} - {pessoa.Nome}");
//}