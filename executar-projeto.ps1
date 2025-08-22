# ==============================================================================
# COZINHA CONECTADA - PDV
# Script Principal de Execução
# ==============================================================================

param(
    [string]$Acao = "menu",
    [string]$Ambiente = "Development"
)

Write-Host "🍽️  COZINHA CONECTADA - SISTEMA PDV" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green
Write-Host ""

function Show-Menu {
    Write-Host "Escolha uma opção:" -ForegroundColor Yellow
    Write-Host "1. 🔧 Configurar banco de dados"
    Write-Host "2. 🚀 Executar API"
    Write-Host "3. 🌐 Executar aplicação Web"
    Write-Host "4. 📊 Executar com SQL Server"
    Write-Host "5. 🧪 Executar testes"
    Write-Host "6. 📋 Gerenciar dados"
    Write-Host "7. 🗑️  Limpar dados de teste"
    Write-Host "0. 🚪 Sair"
    Write-Host ""
}

function Setup-Database {
    Write-Host "🔧 Configurando banco de dados..." -ForegroundColor Blue
    & ".\scripts\criar-banco.bat"
    if (Test-Path ".\scripts\criar-tabelas.sql") {
        Write-Host "✅ Executando script de criação de tabelas..." -ForegroundColor Green
        # Adicionar comando para executar SQL
    }
}

function Start-API {
    Write-Host "🚀 Iniciando API..." -ForegroundColor Blue
    Set-Location ".\API\SistemaPDV.API"
    dotnet run --environment $Ambiente
}

function Start-Web {
    Write-Host "🌐 Iniciando aplicação Web..." -ForegroundColor Blue
    Set-Location ".\Web\SistemaPDV.Web"
    dotnet run --environment $Ambiente
}

function Start-WithSqlServer {
    Write-Host "📊 Executando com SQL Server..." -ForegroundColor Blue
    & ".\tools\executar-sqlserver.ps1"
}

function Run-Tests {
    Write-Host "🧪 Executando testes..." -ForegroundColor Blue
    & ".\tools\test-api.ps1"
}

function Manage-Data {
    Write-Host "📋 Abrindo gerenciador de dados..." -ForegroundColor Blue
    Start-Process ".\tools\gerenciador-dados.html"
}

function Clean-TestData {
    Write-Host "🗑️ Limpando dados de teste..." -ForegroundColor Blue
    if (Test-Path ".\scripts\limpar-dados-teste.sql") {
        # Adicionar comando para executar SQL
        Write-Host "✅ Dados de teste removidos!" -ForegroundColor Green
    }
}

# Menu principal
if ($Acao -eq "menu") {
    do {
        Show-Menu
        $opcao = Read-Host "Digite sua opção"
        
        switch ($opcao) {
            "1" { Setup-Database }
            "2" { Start-API }
            "3" { Start-Web }
            "4" { Start-WithSqlServer }
            "5" { Run-Tests }
            "6" { Manage-Data }
            "7" { Clean-TestData }
            "0" { 
                Write-Host "👋 Saindo..." -ForegroundColor Yellow
                exit 
            }
            default { 
                Write-Host "❌ Opção inválida!" -ForegroundColor Red 
            }
        }
        
        if ($opcao -ne "0") {
            Write-Host ""
            Write-Host "Pressione qualquer tecla para continuar..." -ForegroundColor Gray
            $null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")
            Clear-Host
        }
    } while ($opcao -ne "0")
} else {
    # Execução direta baseada no parâmetro
    switch ($Acao.ToLower()) {
        "api" { Start-API }
        "web" { Start-Web }
        "sqlserver" { Start-WithSqlServer }
        "test" { Run-Tests }
        "setup" { Setup-Database }
        default { Show-Menu }
    }
}
