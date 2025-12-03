# step by step

inserir as seguintes linhas no projeto csproj;

<PackAsTool>true</PackAsTool>
<ToolCommandName>maia</ToolCommandName>
<PackageOutputPath>./nupkg</PackageOutputPath>

depois digitar no cmd do projeto

dotnet pack;
ou
dotnet pack -c Release (sugestão do copilot) 

dotnet tool install --global --add-source ./nupkg wolfish.maia

a partir desse momento já da pra invocar a aplicação de qq lugar do sistema operacional


download
file:///C:/Users/renat/source/repos/WolfishTools/Docs/wolfish-tools.zip

file:///C:/Users/renat/source/repos/WolfishTools/Docs/app.zip