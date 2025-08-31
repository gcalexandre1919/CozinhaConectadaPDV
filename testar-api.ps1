#!/usr/bin/env pwsh

Write-Host "=== Testando Sistema PDV - API ===" -ForegroundColor Green

# Iniciar a API em background
Write-Host "Iniciando API..." -ForegroundColor Yellow
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"API\SistemaPDV.API\SistemaPDV.API.csproj`"" -PassThru -WindowStyle Hidden

# Aguardar a API inicializar
Write-Host "Aguardando API inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

try {
    # Testar endpoint de status básico
    Write-Host "Testando endpoint básico..." -ForegroundColor Cyan
    $response = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos" -Method GET -TimeoutSec 10
    Write-Host "Status da resposta: $($response.StatusCode)" -ForegroundColor Green
    Write-Host "Conteúdo da resposta: $($response.Content)" -ForegroundColor White
    
    # Testar outros endpoints
    Write-Host "`nTestando clientes..." -ForegroundColor Cyan
    $clientesResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/clientes" -Method GET -TimeoutSec 10
    Write-Host "Clientes - Status: $($clientesResponse.StatusCode)" -ForegroundColor Green
    
    Write-Host "`nTestando produtos..." -ForegroundColor Cyan
    $produtosResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/produtos" -Method GET -TimeoutSec 10
    Write-Host "Produtos - Status: $($produtosResponse.StatusCode)" -ForegroundColor Green
    
    Write-Host "`n=== SUCESSO! API está funcionando ===" -ForegroundColor Green
    
} catch {
    Write-Host "Erro ao testar API: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Verifique se a API iniciou corretamente." -ForegroundColor Yellow
} finally {
    # Parar o processo da API
    if ($apiProcess -and !$apiProcess.HasExited) {
        Write-Host "`nParando API..." -ForegroundColor Yellow
        $apiProcess.Kill()
        $apiProcess.WaitForExit()
    }
}

Write-Host "`nTeste concluído." -ForegroundColor Cyan
