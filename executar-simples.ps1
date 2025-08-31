# ==============================================================================
# COZINHA CONECTADA - PDV
# Script Principal de Execucao
# ==============================================================================

param(
    [string]$Acao = "menu",
    [string]$Ambiente = "Development"
)

Write-Host "COZINHA CONECTADA - SISTEMA PDV" -ForegroundColor Green
Write-Host "===============================" -ForegroundColor Green
Write-Host ""

function Show-Menu {
    Write-Host "Escolha uma opcao:" -ForegroundColor Yellow
    Write-Host "1. Configurar banco de dados"
    Write-Host "2. Executar API"
    Write-Host "3. Executar aplicacao Web"
    Write-Host "4. Executar com SQL Server"
    Write-Host "5. Executar testes"
    Write-Host "0. Sair"
    Write-Host ""
}

function Start-API {
    Write-Host "Iniciando API..." -ForegroundColor Blue
    if (Test-Path ".\API\SistemaPDV.API") {
        Set-Location ".\API\SistemaPDV.API"
        dotnet run --environment $Ambiente
    } else {
        Write-Host "Erro: Pasta da API nao encontrada!" -ForegroundColor Red
    }
}

function Start-Web {
    Write-Host "Iniciando aplicacao Web..." -ForegroundColor Blue
    if (Test-Path ".\Web\SistemaPDV.Web") {
        Set-Location ".\Web\SistemaPDV.Web"
        dotnet run --environment $Ambiente
    } else {
        Write-Host "Erro: Pasta da Web nao encontrada!" -ForegroundColor Red
    }
}

# Menu principal
if ($Acao -eq "menu") {
    do {
        Show-Menu
        $opcao = Read-Host "Digite sua opcao"
        
        switch ($opcao) {
            "1" { Write-Host "Configuracao de banco em desenvolvimento..." -ForegroundColor Yellow }
            "2" { Start-API }
            "3" { Start-Web }
            "4" { & ".\executar-sqlserver.ps1" }
            "5" { Write-Host "Testes em desenvolvimento..." -ForegroundColor Yellow }
            "0" { 
                Write-Host "Saindo..." -ForegroundColor Yellow
                exit 
            }
            default { 
                Write-Host "Opcao invalida!" -ForegroundColor Red 
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
    # Execucao direta
    switch ($Acao.ToLower()) {
        "api" { Start-API }
        "web" { Start-Web }
        "sqlserver" { & ".\executar-sqlserver.ps1" }
        default { Show-Menu }
    }
}
