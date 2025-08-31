# Script para executar o sistema completo
Write-Host "Iniciando Sistema PDV Completo..." -ForegroundColor Green

# Parar processos existentes  
Write-Host "Parando processos existentes..." -ForegroundColor Yellow
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue

Start-Sleep -Seconds 2

# Iniciar API
Write-Host "Iniciando API..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-Command", "cd 'C:\Users\GC\Documents\GitHub\CozinhaConectada\API\SistemaPDV.API'; dotnet run --urls='http://localhost:5001'; pause" -WindowStyle Normal

Start-Sleep -Seconds 5

# Testar API
Write-Host "Testando API..." -ForegroundColor Magenta
try {
    $response = Invoke-RestMethod -Uri "http://localhost:5001/api/clientes" -Method GET -TimeoutSec 10
    Write-Host "API funcionando!" -ForegroundColor Green
} catch {
    Write-Host "Erro na API: $($_.Exception.Message)" -ForegroundColor Red
    Read-Host "Pressione Enter para continuar mesmo assim..."
}

# Iniciar Web
Write-Host "Iniciando Web..." -ForegroundColor Cyan
Start-Process powershell -ArgumentList "-Command", "cd 'C:\Users\GC\Documents\GitHub\CozinhaConectada\Web\SistemaPDV.Web'; dotnet run --urls='http://localhost:5000'; pause" -WindowStyle Normal

Start-Sleep -Seconds 3

# Abrir browser
Write-Host "Abrindo browser..." -ForegroundColor Green
Start-Process "http://localhost:5000"

Write-Host ""
Write-Host "Sistema iniciado com sucesso!" -ForegroundColor Green
Write-Host "API: http://localhost:5001" -ForegroundColor White
Write-Host "Web: http://localhost:5000" -ForegroundColor White
Write-Host ""
Write-Host "Pressione Enter para fechar os servicos..." -ForegroundColor Yellow
Read-Host

# Parar processos
Write-Host "Parando servicos..." -ForegroundColor Red
Get-Process -Name "dotnet" -ErrorAction SilentlyContinue | Stop-Process -Force -ErrorAction SilentlyContinue
Write-Host "Servicos parados!" -ForegroundColor Green
