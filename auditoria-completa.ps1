#!/usr/bin/env pwsh

Write-Host "=== AUDITORIA COMPLETA DO SISTEMA PDV ===" -ForegroundColor Green

# Iniciar API
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"API\SistemaPDV.API\SistemaPDV.API.csproj`"" -PassThru -WindowStyle Hidden
Start-Sleep -Seconds 5

try {
    Write-Host "`n1. TESTANDO CADASTRO DE CLIENTES" -ForegroundColor Cyan
    
    # Criar cliente 1
    $cliente1 = @{
        nome = "João Silva"
        telefone = "11999999999"
        email = "joao@teste.com"
        cpf = "12345678901"
        observacoes = "Cliente teste 1"
    } | ConvertTo-Json -Compress
    
    $response1 = Invoke-WebRequest -Uri "http://localhost:5000/api/clientes" -Method POST -Body $cliente1 -ContentType "application/json"
    Write-Host "✓ Cliente 1 criado - Status: $($response1.StatusCode)" -ForegroundColor Green
    $clienteId1 = ($response1.Content | ConvertFrom-Json).id
    
    # Criar cliente 2 com MESMO telefone
    $cliente2 = @{
        nome = "Maria Silva"
        telefone = "11999999999"  # MESMO TELEFONE!
        email = "maria@teste.com"
        cpf = "98765432100"
        observacoes = "Cliente teste 2 - mesmo telefone"
    } | ConvertTo-Json -Compress
    
    $response2 = Invoke-WebRequest -Uri "http://localhost:5000/api/clientes" -Method POST -Body $cliente2 -ContentType "application/json"
    Write-Host "✓ Cliente 2 criado com mesmo telefone - Status: $($response2.StatusCode)" -ForegroundColor Green
    $clienteId2 = ($response2.Content | ConvertFrom-Json).id
    
    Write-Host "`n2. TESTANDO CADASTRO DE PRODUTOS" -ForegroundColor Cyan
    
    # Criar categoria
    $categoria = @{
        nome = "Pratos Principais"
        descricao = "Categoria de teste"
    } | ConvertTo-Json -Compress
    
    $catResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/categorias" -Method POST -Body $categoria -ContentType "application/json"
    Write-Host "✓ Categoria criada - Status: $($catResponse.StatusCode)" -ForegroundColor Green
    $categoriaId = ($catResponse.Content | ConvertFrom-Json).id
    
    # Criar produto
    $produto = @{
        nome = "Hambúrguer Especial"
        descricao = "Delicioso hambúrguer artesanal"
        preco = 25.90
        categoriaId = $categoriaId
        ativo = $true
    } | ConvertTo-Json -Compress
    
    $prodResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/produtos" -Method POST -Body $produto -ContentType "application/json"
    Write-Host "✓ Produto criado - Status: $($prodResponse.StatusCode)" -ForegroundColor Green
    $produtoId = ($prodResponse.Content | ConvertFrom-Json).id
    
    Write-Host "`n3. TESTANDO CADASTRO DE PEDIDOS" -ForegroundColor Cyan
    
    # Criar pedido com itens
    $pedido = @{
        clienteId = $clienteId1
        tipo = 1  # Retirada
        observacoes = "Pedido de teste"
        itens = @(
            @{
                produtoId = $produtoId
                quantidade = 2
                observacoes = "Sem cebola"
            }
        )
    } | ConvertTo-Json -Depth 5 -Compress
    
    $pedidoResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos" -Method POST -Body $pedido -ContentType "application/json"
    Write-Host "✓ Pedido criado - Status: $($pedidoResponse.StatusCode)" -ForegroundColor Green
    $pedidoId = ($pedidoResponse.Content | ConvertFrom-Json).id
    
    Write-Host "`n4. TESTANDO IMPRESSÃO DE PEDIDOS" -ForegroundColor Cyan
    
    $imprimirResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos/$pedidoId/imprimir" -Method POST
    Write-Host "✓ Impressão do pedido - Status: $($imprimirResponse.StatusCode)" -ForegroundColor Green
    
    Write-Host "`n5. TESTANDO RELATÓRIOS" -ForegroundColor Cyan
    
    # Relatório por período
    $dataInicio = (Get-Date).ToString("yyyy-MM-dd")
    $dataFim = (Get-Date).ToString("yyyy-MM-dd")
    $relatorioResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos/por-periodo?dataInicio=$dataInicio`&dataFim=$dataFim" -Method GET
    Write-Host "✓ Relatório por período - Status: $($relatorioResponse.StatusCode)" -ForegroundColor Green
    
    # Relatório por cliente
    $relatorioCliente = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos/por-cliente/$clienteId1" -Method GET
    Write-Host "✓ Relatório por cliente - Status: $($relatorioCliente.StatusCode)" -ForegroundColor Green
    
    Write-Host "`n6. TESTANDO MÚLTIPLOS RESTAURANTES" -ForegroundColor Cyan
    
    # Verificar restaurantes existentes
    try {
        $restaurantesResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/restaurantes" -Method GET
        Write-Host "✓ Endpoint de restaurantes - Status: $($restaurantesResponse.StatusCode)" -ForegroundColor Green
    } catch {
        Write-Host "⚠️ Endpoint de restaurantes não implementado ainda" -ForegroundColor Yellow
    }
    
    Write-Host "`n=== RESUMO DA AUDITORIA ===" -ForegroundColor Green
    Write-Host "✓ Cadastro de clientes: FUNCIONANDO" -ForegroundColor Green
    Write-Host "✓ Múltiplos clientes com mesmo telefone: PERMITIDO" -ForegroundColor Green
    Write-Host "✓ Cadastro de produtos: FUNCIONANDO" -ForegroundColor Green
    Write-Host "✓ Cadastro de pedidos: FUNCIONANDO" -ForegroundColor Green
    Write-Host "✓ Impressão de pedidos: FUNCIONANDO (simulada)" -ForegroundColor Green
    Write-Host "✓ Relatórios básicos: FUNCIONANDO" -ForegroundColor Green
    Write-Host "⚠️ Múltiplos restaurantes: ENDPOINT NÃO ENCONTRADO" -ForegroundColor Yellow
    Write-Host "⚠️ Geração de PDF: NÃO TESTADO" -ForegroundColor Yellow
    
} catch {
    Write-Host "❌ ERRO: $($_.Exception.Message)" -ForegroundColor Red
    Write-Host "Detalhes: $($_.Exception.Response)" -ForegroundColor Red
} finally {
    if ($apiProcess -and !$apiProcess.HasExited) {
        $apiProcess.Kill()
        $apiProcess.WaitForExit()
    }
}
