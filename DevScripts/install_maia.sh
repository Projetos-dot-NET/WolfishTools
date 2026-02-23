#!/bin/bash

# Define as variáveis (sem espaços ao redor do =)
PACKAGE_NAME="wolfish.maia"
NUGET_OUTPUT_DIR="../DevScripts/nupkg"

echo -e "\n=== 1. Rebuildando Wolfish.Maia em modo Release... ===\n"

# Volta um diretório, limpa e builda
cd ..
dotnet clean
dotnet build

cd Wolfish.Maia

echo -e "\n=== 2. Empacotando Wolfish.Maia em modo Release... ===\n"

# Empacota o projeto
dotnet pack -c Release --output "$NUGET_OUTPUT_DIR"

# Verifica se o último comando (dotnet pack) falhou
if [ $? -ne 0 ]; then
    echo -e "\nERRO: Falha ao empacotar o projeto. Verifique o log acima."
    exit 1
fi

echo -e "\nEmpacotamento concluído com sucesso! Pacote gerado em: $NUGET_OUTPUT_DIR\n"
echo "=== 3. Tentando desinstalar a versão global anterior (se existir)... ==="

# Desinstala a versão global (o || true evita que o script pare se ela não existir)
dotnet tool uninstall --global $PACKAGE_NAME || true

echo -e "\n=== 4. Instalando a nova versão globalmente a partir da fonte local... ===\n"

# Instala a nova versão
dotnet tool install --global --add-source "$NUGET_OUTPUT_DIR" $PACKAGE_NAME

if [ $? -ne 0 ]; then
    echo -e "\nERRO: Falha ao instalar a ferramenta globalmente."
    echo "A ferramenta já pode estar instalada ou o nome do pacote está incorreto."
    exit 1
fi

echo -e "\nSUCESSO! A ferramenta $PACKAGE_NAME já pode ser invocada de qualquer lugar."
echo -e "\nTente digitar: maia welcome\n"

# Simula o 'pause' do Windows
read -p "Pressione [Enter] para finalizar..."