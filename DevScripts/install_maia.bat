@echo off
setlocal
set PACKAGE_NAME=wolfish.maia
set NUGET_OUTPUT_DIR=.\nupkg

echo.
echo === 1. Empacotando a aplicação em modo Release... ===
echo.

cd ..\Wolfish.Maia

dotnet pack -c Release --output %NUGET_OUTPUT_DIR%

IF ERRORLEVEL 1 (
    echo.
    echo ERRO: Falha ao empacotar o projeto. Verifique o log acima.
    goto :eof
)

echo.
echo Empacotamento concluido com sucesso! Pacote gerado em: %NUGET_OUTPUT_DIR%
echo.
echo === 2. Tentando desinstalar a versão global anterior (se existir)... ===
echo.

dotnet tool uninstall --global %PACKAGE_NAME%

echo.
echo === 3. Instalando a nova versão globalmente a partir da fonte local... ===
echo.

dotnet tool install --global --add-source %NUGET_OUTPUT_DIR% %PACKAGE_NAME%

IF ERRORLEVEL 1 (
    echo.
    echo ERRO: Falha ao instalar a ferramenta globalmente.
    echo A ferramenta ja pode estar instalada ou o nome do pacote esta incorreto.
    goto :eof
)

echo.
echo SUCESSO! A ferramenta %PACKAGE_NAME% ja pode ser invocada de qualquer lugar.
echo.
echo Tente digitar: maia welcome

pause
endlocal
