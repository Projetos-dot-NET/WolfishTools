# Força o console do PowerShell a renderizar caracteres em UTF-8 corretamente
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "Instalando Wolfish.Maia, sua Assistente Integrada Automatizada..." -ForegroundColor Cyan

# Verifica se o .NET SDK está instalado no computador do usuário
if ((Get-Command dotnet -ErrorAction SilentlyContinue) -eq $null) {
    Write-Error "O .NET SDK não foi encontrado. Instale o .NET 10 para rodar a MAIA."
    Exit
}

# Executa o comando oficial de instalação global do NuGet
dotnet tool install --global Wolfish.Maia --version 0.0.7

if ($LASTEXITCODE -eq 0) {
    Write-Host "`nSucesso! Wolfish.Maia instalada com êxito." -ForegroundColor Green
    Write-Host "Digite 'maia welcome' para começar." -ForegroundColor Yellow
} else {
    Write-Error "Ocorreu um erro durante a instalação através do NuGet."
}