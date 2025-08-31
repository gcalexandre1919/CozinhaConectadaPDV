# ==============================================================================
# COZINHA CONECTADA - PDV
# Script Principal de Execucao
# ==============================================================================

param(
    [string]$Acao = "menu",
    [string]$Ambiente = "Development"
)

Write-Host "🍽️ COZINHA CONECTADA - SISTEMA PDV" -ForegroundColor Green
Write-Host "=================================" -ForegroundColor Green
Write-Host ""

function Show-Menu {
    Write-Host "Escolha uma opcao:" -ForegroundColor Yellow
    Write-Host "1. 🔧 Configurar banco de dados"
    Write-Host "2. 🚀 Executar API"
    Write-Host "3. 🌐 Executar aplicacao Web"
    Write-Host "4. 📊 Executar com SQL Server"
    Write-Host "5. 🧪 Executar testes"
    Write-Host "6. 📋 Gerenciar dados"
    Write-Host "7. 🗑️ Limpar dados de teste"
    Write-Host "0. 🚪 Sair"
    Write-Host ""
}

function Setup-Database {
    Write-Host "🔧 Configurando banco de dados..." -ForegroundColor Blue
    if (Test-Path ".\scripts\criar-banco.bat") {
        & ".\scripts\criar-banco.bat"
    }
    if (Test-Path ".\scripts\criar-tabelas.sql") {
        Write-Host "✅ Script de tabelas encontrado!" -ForegroundColor Green
    }
}

function Start-API {
    Write-Host "🚀 Iniciando API..." -ForegroundColor Blue
    if (Test-Path ".\API\SistemaPDV.API") {
        Set-Location ".\API\SistemaPDV.API"
        dotnet run --environment $Ambiente
    } else {
        Write-Host "❌ Pasta da API nao encontrada!" -ForegroundColor Red
    }
}

function Start-Web {
    Write-Host "🌐 Iniciando aplicacao Web..." -ForegroundColor Blue
    if (Test-Path ".\Web\SistemaPDV.Web") {
        Set-Location ".\Web\SistemaPDV.Web"
        dotnet run --environment $Ambiente
    } else {
        Write-Host "❌ Pasta da Web nao encontrada!" -ForegroundColor Red
    }
}

function Start-WithSqlServer {
    Write-Host "📊 Executando com SQL Server..." -ForegroundColor Blue
    & ".\executar-sqlserver.ps1"
}

function Run-Tests {
    Write-Host "🧪 Executando testes..." -ForegroundColor Blue
    if (Test-Path ".\tools\test-api.ps1") {
        & ".\tools\test-api.ps1"
    } else {
        Write-Host "⚠️ Script de testes nao encontrado!" -ForegroundColor Yellow
    }
}

function Manage-Data {
    Write-Host "📋 Abrindo gerenciador de dados..." -ForegroundColor Blue
    if (Test-Path ".\tools\gerenciador-dados.html") {
        Start-Process ".\tools\gerenciador-dados.html"
    } else {
        Write-Host "❌ Gerenciador de dados nao encontrado!" -ForegroundColor Red
    }
}

function Clean-TestData {
    Write-Host "🗑️ Limpando dados de teste..." -ForegroundColor Blue
    if (Test-Path ".\scripts\limpar-dados-teste.sql") {
        Write-Host "✅ Script de limpeza encontrado!" -ForegroundColor Green
    } else {
        Write-Host "❌ Script de limpeza nao encontrado!" -ForegroundColor Red
    }
}

# Menu principal
if ($Acao -eq "menu") {
    do {
        Show-Menu
        $opcao = Read-Host "Digite sua opcao"
        
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
                Write-Host "❌ Opcao invalida!" -ForegroundColor Red 
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
    # Execucao direta baseada no parametro
    switch ($Acao.ToLower()) {
        "api" { Start-API }
        "web" { Start-Web }
        "sqlserver" { Start-WithSqlServer }
        "test" { Run-Tests }
        "setup" { Setup-Database }
        default { Show-Menu }
    }
}
