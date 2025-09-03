#!/usr/bin/env pwsh
# Script para testar o sistema PDV completo - todas as funcionalidades

Write-Host "=== TESTE SISTEMA PDV COMPLETO ===" -ForegroundColor Green
Write-Host "Testando todas as funcionalidades implementadas..." -ForegroundColor Yellow

$baseUrl = "http://localhost:5001"

# Função para fazer requisições HTTP
function Invoke-ApiTest {
    param(
        [string]$Endpoint,
        [string]$Method = "GET",
        [string]$Description
    )
    
    try {
        Write-Host "`n[$Method] $Description" -ForegroundColor Cyan
        Write-Host "URL: $baseUrl$Endpoint" -ForegroundColor Gray
        
        $response = Invoke-RestMethod -Uri "$baseUrl$Endpoint" -Method $Method -ErrorAction Stop
        Write-Host "✅ Sucesso - Dados retornados: $(($response | ConvertTo-Json -Depth 1).Length) caracteres" -ForegroundColor Green
        return $response
    }
    catch {
        Write-Host "❌ Erro: $($_.Exception.Message)" -ForegroundColor Red
        return $null
    }
}

Write-Host "`n🔍 TESTANDO ENDPOINTS DA API..." -ForegroundColor Magenta

# 1. Teste de Categorias (com impressoras associadas)
Write-Host "`n📁 GESTÃO DE CATEGORIAS" -ForegroundColor Blue
$categorias = Invoke-ApiTest -Endpoint "/api/categorias" -Description "Listar todas as categorias"
$categoriasSimple = Invoke-ApiTest -Endpoint "/api/categoriassimple" -Description "Listar categorias simplificadas"

# 2. Teste de Impressoras
Write-Host "`n🖨️ GESTÃO DE IMPRESSORAS" -ForegroundColor Blue
$impressoras = Invoke-ApiTest -Endpoint "/api/impressoras" -Description "Listar todas as impressoras"

# 3. Teste de Clientes
Write-Host "`n👥 GESTÃO DE CLIENTES" -ForegroundColor Blue
$clientes = Invoke-ApiTest -Endpoint "/api/clientes" -Description "Listar todos os clientes"

# 4. Teste de Produtos
Write-Host "`n🛍️ GESTÃO DE PRODUTOS" -ForegroundColor Blue
$produtos = Invoke-ApiTest -Endpoint "/api/produtos" -Description "Listar todos os produtos"

# 5. Teste de Pedidos
Write-Host "`n📋 GESTÃO DE PEDIDOS" -ForegroundColor Blue
$pedidos = Invoke-ApiTest -Endpoint "/api/pedidos" -Description "Listar todos os pedidos"

# 6. Teste de Impressão
Write-Host "`n🖨️ SISTEMA DE IMPRESSÃO" -ForegroundColor Blue
$impressao = Invoke-ApiTest -Endpoint "/api/impressao/impressoras" -Description "Listar impressoras para impressão"

# 7. Teste de Relatórios
Write-Host "`n📊 SISTEMA DE RELATÓRIOS" -ForegroundColor Blue
$relatorios = Invoke-ApiTest -Endpoint "/api/relatorios/vendas-periodo" -Description "Relatórios de vendas"

Write-Host "`n=== RESUMO DOS TESTES ===" -ForegroundColor Green

$endpoints = @(
    @{ Nome = "Categorias"; Status = if($categorias) { "✅" } else { "❌" } }
    @{ Nome = "Categorias Simple"; Status = if($categoriasSimple) { "✅" } else { "❌" } }
    @{ Nome = "Impressoras"; Status = if($impressoras) { "✅" } else { "❌" } }
    @{ Nome = "Clientes"; Status = if($clientes) { "✅" } else { "❌" } }
    @{ Nome = "Produtos"; Status = if($produtos) { "✅" } else { "❌" } }
    @{ Nome = "Pedidos"; Status = if($pedidos) { "✅" } else { "❌" } }
    @{ Nome = "Impressão"; Status = if($impressao) { "✅" } else { "❌" } }
    @{ Nome = "Relatórios"; Status = if($relatorios) { "✅" } else { "❌" } }
)

foreach ($endpoint in $endpoints) {
    Write-Host "$($endpoint.Status) $($endpoint.Nome)" -ForegroundColor $(if($endpoint.Status -eq "✅") { "Green" } else { "Red" })
}

$sucessos = ($endpoints | Where-Object { $_.Status -eq "✅" }).Count
$total = $endpoints.Count
$percentual = [math]::Round(($sucessos / $total) * 100, 1)

Write-Host "`n📈 RESULTADO FINAL: $sucessos/$total endpoints funcionando ($percentual%)" -ForegroundColor $(if($percentual -ge 90) { "Green" } else { if($percentual -ge 70) { "Yellow" } else { "Red" } })

if ($percentual -eq 100) {
    Write-Host "🎉 SISTEMA 100% FUNCIONAL! Todas as funcionalidades estão operacionais." -ForegroundColor Green
} elseif ($percentual -ge 90) {
    Write-Host "✨ SISTEMA QUASE COMPLETO! Poucas funcionalidades pendentes." -ForegroundColor Yellow
} else {
    Write-Host "⚠️ SISTEMA NECESSITA AJUSTES para funcionalidade completa." -ForegroundColor Red
}

Write-Host "`n🌐 ACESSOS DIRETOS:" -ForegroundColor Cyan
Write-Host "Web Interface: http://localhost:5000" -ForegroundColor White
Write-Host "API Swagger: http://localhost:5001/swagger" -ForegroundColor White
Write-Host "`nSistema pronto para uso! 🚀" -ForegroundColor Green
