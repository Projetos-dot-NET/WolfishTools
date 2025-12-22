winget install -e --id Microsoft.AzureCLI

az --version

az login

az account list --output table

az account set --subscription "Nome da Sua Assinatura"

az extension add --name azure-devops

# Criar um grupo chamado "MeuGrupo" no leste dos EUA
az group create --name MeuGrupo --location eastus

# Listar todos os seus grupos
az group list --output table

# Estando na pasta do seu projeto .NET
az webapp up --name NomeUnicoDoSeuSite --resource-group MeuGrupoDeTestes --runtime "DOTNET|8.0"

az devops configure --defaults organization=https://dev.azure.com/SUA_ORG project=SEU_PROJETO

az repos pr create --title "Minha Feature" --description "Descrição aqui" --source-branch developer --target-branch main

az repos pr create --title "Meu PR via CLI" --description "Feito pelo terminal" --source-branch developer --target-branch master

az find "create a vm"


