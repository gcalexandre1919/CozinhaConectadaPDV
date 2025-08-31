#!/usr/bin/env pwsh

Write-Host "=== EXECUTANDO SISTEMA PDV ===" -ForegroundColor Green

# Listar arquivos importantes
Write-Host "`n📁 Estrutura do Projeto:" -ForegroundColor Cyan
Get-ChildItem -Path "API\SistemaPDV.API\Controllers" -Filter "*.cs" | ForEach-Object { 
    Write-Host "   ✓ $($_.Name)" -ForegroundColor Green 
}

Write-Host "`n📁 Serviços Implementados:" -ForegroundColor Cyan
Get-ChildItem -Path "Infrastructure\SistemaPDV.Infrastructure\Services" -Filter "*.cs" | ForEach-Object { 
    Write-Host "   ✓ $($_.Name)" -ForegroundColor Green 
}

Write-Host "`n📁 Entidades Criadas:" -ForegroundColor Cyan
Get-ChildItem -Path "Core\SistemaPDV.Core\Entities" -Filter "*.cs" | ForEach-Object { 
    Write-Host "   ✓ $($_.Name)" -ForegroundColor Green 
}

Write-Host "`n🔧 Compilando Projeto..." -ForegroundColor Yellow
$buildResult = dotnet build "API\SistemaPDV.API\SistemaPDV.API.csproj" --verbosity quiet
if ($LASTEXITCODE -eq 0) {
    Write-Host "✅ Compilação bem-sucedida!" -ForegroundColor Green
} else {
    Write-Host "❌ Erro na compilação!" -ForegroundColor Red
    exit 1
}

Write-Host "`n🚀 Iniciando Sistema..." -ForegroundColor Yellow
Write-Host "API será executada em: http://localhost:5000" -ForegroundColor Cyan
Write-Host "Pressione Ctrl+C para parar" -ForegroundColor Yellow

# Executar API (vai ficar em primeiro plano)
dotnet run --project "API\SistemaPDV.API\SistemaPDV.API.csproj"
