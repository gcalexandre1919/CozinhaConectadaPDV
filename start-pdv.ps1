#!/usr/bin/env powershell
param(
    [switch]$Clean,
    [switch]$ApiOnly,
    [switch]$WebOnly,
    [switch]$Help
)

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "  Sistema PDV - Cozinha Conectada" -ForegroundColor Green
Write-Host "========================================" -ForegroundColor Cyan

if ($Help) {
    Write-Host "Uso:" -ForegroundColor Yellow
    Write-Host "  .\start-pdv.ps1          # Executa API + Web"
    Write-Host "  .\start-pdv.ps1 -ApiOnly # Apenas API"
    Write-Host "  .\start-pdv.ps1 -WebOnly # Apenas Web"
    Write-Host "  .\start-pdv.ps1 -Clean   # Limpa e reconstrói"
    Write-Host "  .\start-pdv.ps1 -Help    # Esta ajuda"
    return
}

if ($Clean) {
    Write-Host "Limpando solution..." -ForegroundColor Yellow
    dotnet clean SistemaPDV.sln
    Remove-Item -Recurse -Force -ErrorAction SilentlyContinue "*/bin", "*/obj"
}

Write-Host "Fazendo build da solution..." -ForegroundColor Yellow
$buildResult = dotnet build SistemaPDV.sln
if ($LASTEXITCODE -ne 0) {
    Write-Host "Erro no build! Verifique os erros acima." -ForegroundColor Red
    return
}

Write-Host "Build concluido com sucesso!" -ForegroundColor Green

if ($ApiOnly) {
    Write-Host "Iniciando apenas a API..." -ForegroundColor Blue
    Write-Host "API estara disponivel em: http://localhost:5001" -ForegroundColor Cyan
    Write-Host "Swagger disponivel em: http://localhost:5001/swagger" -ForegroundColor Cyan
    Set-Location "API\SistemaPDV.API"
    dotnet run
}
elseif ($WebOnly) {
    Write-Host "Iniciando apenas a Web..." -ForegroundColor Blue
    Write-Host "Web estara disponivel em: http://localhost:5000" -ForegroundColor Cyan
    Set-Location "Web\SistemaPDV.Web"
    dotnet run
}
else {
    Write-Host "Iniciando API e Web..." -ForegroundColor Blue
    Write-Host ""
    Write-Host "Enderecos disponiveis:" -ForegroundColor Cyan
    Write-Host "  Web App: http://localhost:5000" -ForegroundColor White
    Write-Host "  API REST: http://localhost:5001" -ForegroundColor White
    Write-Host "  Swagger: http://localhost:5001/swagger" -ForegroundColor White
    Write-Host ""
    Write-Host "Para parar os servicos, pressione Ctrl+C em ambos os terminais" -ForegroundColor Yellow
    Write-Host ""

    # Inicia a API em background
    Write-Host "Iniciando API..." -ForegroundColor Blue
    Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '$PWD\API\SistemaPDV.API'; Write-Host 'API iniciando...' -ForegroundColor Green; dotnet run --urls 'http://localhost:5001'"
    
    # Aguarda API inicializar
    Write-Host "Aguardando API inicializar..." -ForegroundColor Yellow
    Start-Sleep -Seconds 8
    
    # Tenta criar dados iniciais automaticamente
    try {
        Write-Host "Criando dados iniciais automaticamente..." -ForegroundColor Yellow
        $response = Invoke-WebRequest -Uri "http://localhost:5001/api/seed/criar-dados-iniciais" -Method POST -UseBasicParsing -TimeoutSec 10 -ErrorAction Stop
        Write-Host "✅ Dados iniciais criados com sucesso!" -ForegroundColor Green
        Write-Host "   Login: admin@sistema.com / 123456" -ForegroundColor Cyan
    }
    catch {
        Write-Host "⚠️  Dados iniciais não criados automaticamente" -ForegroundColor Yellow
        Write-Host "   Execute manualmente: http://localhost:5001/swagger" -ForegroundColor Cyan
        Write-Host "   Endpoint: POST /api/seed/criar-dados-iniciais" -ForegroundColor Cyan
    }
    
    Write-Host ""
    Write-Host "Iniciando Web..." -ForegroundColor Blue
    Set-Location "Web\SistemaPDV.Web"
    dotnet run --urls "http://localhost:5000"
}
