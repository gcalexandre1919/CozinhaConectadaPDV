param(
    [string]$ApiUrl = "http://localhost:5000"
)

Write-Host "🚀 Criando dados de exemplo no Sistema PDV..." -ForegroundColor Green

try {
    # Criar dados de exemplo
    $createUrl = "$ApiUrl/api/Seed/dados-exemplo"
    Write-Host "📤 Enviando requisição para: $createUrl" -ForegroundColor Yellow
    
    $response = Invoke-RestMethod -Uri $createUrl -Method POST -ContentType "application/json"
    
    Write-Host "✅ Dados criados com sucesso!" -ForegroundColor Green
    Write-Host "📋 Resposta da API:" -ForegroundColor Cyan
    $response | ConvertTo-Json -Depth 3 | Write-Host
    
    # Verificar status após criação
    Write-Host "`n🔍 Verificando status do banco..." -ForegroundColor Yellow
    $statusUrl = "$ApiUrl/api/Seed/status"
    $status = Invoke-RestMethod -Uri $statusUrl -Method GET
    
    Write-Host "📊 Status atual do banco:" -ForegroundColor Cyan
    $status | ConvertTo-Json -Depth 3 | Write-Host
    
    # Testar endpoint de produtos
    Write-Host "`n🍽️ Testando endpoint de produtos..." -ForegroundColor Yellow
    $produtosUrl = "$ApiUrl/api/Produtos"
    $produtos = Invoke-RestMethod -Uri $produtosUrl -Method GET
    
    Write-Host "📋 Produtos encontrados: $($produtos.Count)" -ForegroundColor Green
    if ($produtos.Count -gt 0) {
        Write-Host "🎉 Sistema funcionando corretamente!" -ForegroundColor Green
    }
    
} catch {
    Write-Host "❌ Erro ao criar dados: $($_.Exception.Message)" -ForegroundColor Red
    if ($_.Exception.Response) {
        Write-Host "📄 Status HTTP: $($_.Exception.Response.StatusCode)" -ForegroundColor Red
    }
}

Write-Host "`n✨ Script finalizado!" -ForegroundColor Green
