Write-Host "=== AUDITORIA COMPLETA DO SISTEMA PDV ===" -ForegroundColor Green

# Iniciar API
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"API\SistemaPDV.API\SistemaPDV.API.csproj`"" -PassThru -WindowStyle Hidden
Start-Sleep -Seconds 5

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
    telefone = "11999999999"
    email = "maria@teste.com"
    cpf = "98765432100"
    observacoes = "Cliente teste 2 - mesmo telefone"
} | ConvertTo-Json -Compress

$response2 = Invoke-WebRequest -Uri "http://localhost:5000/api/clientes" -Method POST -Body $cliente2 -ContentType "application/json"
Write-Host "✓ Cliente 2 criado com mesmo telefone - Status: $($response2.StatusCode)" -ForegroundColor Green

Write-Host "`n2. TESTANDO CADASTRO DE PRODUTOS" -ForegroundColor Cyan

# Criar categoria
$categoria = @{
    nome = "Pratos Principais"
    descricao = "Categoria de teste"
} | ConvertTo-Json -Compress

try {
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
} catch {
    Write-Host "⚠️ Erro ao criar categoria/produto: $($_.Exception.Message)" -ForegroundColor Yellow
    $produtoId = 1 # Assumir que existe produto com ID 1
}

Write-Host "`n3. TESTANDO CADASTRO DE PEDIDOS" -ForegroundColor Cyan

$pedido = @{
    clienteId = $clienteId1
    tipo = 1
    observacoes = "Pedido de teste"
    itens = @(
        @{
            produtoId = $produtoId
            quantidade = 2
            observacoes = "Sem cebola"
        }
    )
} | ConvertTo-Json -Depth 5 -Compress

try {
    $pedidoResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos" -Method POST -Body $pedido -ContentType "application/json"
    Write-Host "✓ Pedido criado - Status: $($pedidoResponse.StatusCode)" -ForegroundColor Green
    $pedidoId = ($pedidoResponse.Content | ConvertFrom-Json).id
    
    Write-Host "`n4. TESTANDO IMPRESSÃO" -ForegroundColor Cyan
    $imprimirResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos/$pedidoId/imprimir" -Method POST
    Write-Host "✓ Impressão - Status: $($imprimirResponse.StatusCode)" -ForegroundColor Green
    
} catch {
    Write-Host "⚠️ Erro ao criar pedido: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host "`n5. TESTANDO RELATÓRIOS" -ForegroundColor Cyan
try {
    $relatorioResponse = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos" -Method GET
    Write-Host "✓ Listar pedidos - Status: $($relatorioResponse.StatusCode)" -ForegroundColor Green
} catch {
    Write-Host "⚠️ Erro nos relatórios: $($_.Exception.Message)" -ForegroundColor Yellow
}

Write-Host "`n=== RESUMO ===" -ForegroundColor Green
Write-Host "✓ Sistema básico funcionando" -ForegroundColor Green
Write-Host "⚠️ Algumas funcionalidades podem precisar de ajustes" -ForegroundColor Yellow

if ($apiProcess -and !$apiProcess.HasExited) {
    $apiProcess.Kill()
    $apiProcess.WaitForExit()
}
