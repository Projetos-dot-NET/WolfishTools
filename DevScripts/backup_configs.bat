@echo off
chcp 65001 >nul 2>&1
setlocal enabledelayedexpansion

:: ============================================================
:: Script de Backup - appsettings.json e cloudagents.json
:: Cria cópias com timestamp na pasta DevScripts\backups\
:: ============================================================

:: Diretório do script e do projeto
set "SCRIPT_DIR=%~dp0"
set "SOURCE_DIR=%SCRIPT_DIR%..\Wolfish.Maia"
set "BACKUP_DIR=%SCRIPT_DIR%backups"

:: Timestamp para o nome do backup
for /f "tokens=1-3 delims=/" %%a in ("%date%") do set "DT=%%c-%%b-%%a"
for /f "tokens=1-3 delims=:." %%a in ("%time: =0%") do set "TM=%%a-%%b-%%c"
set "TIMESTAMP=%DT%_%TM%"

echo.
echo ============================================
echo   Backup de Configuracoes - Wolfish.Maia
echo ============================================
echo.
echo   Origem:  %SOURCE_DIR%
echo   Destino: %BACKUP_DIR%
echo   Data:    %date% %time%
echo.

:: Cria o diretório de backups se não existir
if not exist "%BACKUP_DIR%" mkdir "%BACKUP_DIR%"

set COPIED=0
set ERRORS=0

:: Backup de appsettings.json
if exist "%SOURCE_DIR%\appsettings.json" (
    copy /Y "%SOURCE_DIR%\appsettings.json" "%BACKUP_DIR%\appsettings_%TIMESTAMP%.json" >nul
    if !errorlevel! equ 0 (
        echo   [OK] appsettings.json -^> appsettings_%TIMESTAMP%.json
        set /a COPIED+=1
    ) else (
        echo   [ERRO] Falha ao copiar appsettings.json
        set /a ERRORS+=1
    )
) else (
    echo   [AVISO] Arquivo nao encontrado: appsettings.json
    set /a ERRORS+=1
)

:: Backup de cloudagents.json
if exist "%SOURCE_DIR%\cloudagents.json" (
    copy /Y "%SOURCE_DIR%\cloudagents.json" "%BACKUP_DIR%\cloudagents_%TIMESTAMP%.json" >nul
    if !errorlevel! equ 0 (
        echo   [OK] cloudagents.json -^> cloudagents_%TIMESTAMP%.json
        set /a COPIED+=1
    ) else (
        echo   [ERRO] Falha ao copiar cloudagents.json
        set /a ERRORS+=1
    )
) else (
    echo   [AVISO] Arquivo nao encontrado: cloudagents.json
    set /a ERRORS+=1
)

echo.
echo --------------------------------------------
echo   Resumo: %COPIED% arquivo(s) copiado(s), %ERRORS% erro(s)
echo --------------------------------------------
echo.

if %ERRORS% equ 0 (
    echo   Backup concluido com sucesso!
) else (
    echo   Backup concluido com %ERRORS% erro(s).
)

echo.
pause
