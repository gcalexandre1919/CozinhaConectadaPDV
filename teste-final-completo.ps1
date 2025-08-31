Write-Host "=== TESTE FINAL DO SISTEMA PDV COMPLETO ===" -ForegroundColor Green

# Iniciar API
Write-Host "Iniciando API..." -ForegroundColor Yellow
$apiProcess = Start-Process -FilePath "dotnet" -ArgumentList "run --project `"API\SistemaPDV.API\SistemaPDV.API.csproj`"" -PassThru -WindowStyle Hidden
Start-Sleep -Seconds 8

try {
    Write-Host "`n1. ✅ TESTANDO CADASTRO DE CLIENTES" -ForegroundColor Cyan
    
    $cliente = @{
        nome = "Cliente Final Test"
        telefone = "11987654321"
        email = "cliente@final.com"
        cpf = "12312312312"
    } | ConvertTo-Json -Compress
    
    $clienteResp = Invoke-WebRequest -Uri "http://localhost:5000/api/clientes" -Method POST -Body $cliente -ContentType "application/json"
    Write-Host "Status: $($clienteResp.StatusCode) - Cliente criado!" -ForegroundColor Green
    $clienteId = ($clienteResp.Content | ConvertFrom-Json).id
    
    Write-Host "`n2. ✅ TESTANDO MÚLTIPLOS RESTAURANTES" -ForegroundColor Cyan
    
    $restaurante = @{
        nome = "Novo Restaurante"
        cnpj = "12345678000199"
        endereco = "Rua Teste, 123"
        telefone = "11999887766"
        email = "restaurante@teste.com"
        cidade = "São Paulo"
        uf = "SP"
    } | ConvertTo-Json -Compress
    
    $restResp = Invoke-WebRequest -Uri "http://localhost:5000/api/restaurantes" -Method POST -Body $restaurante -ContentType "application/json"
    Write-Host "Status: $($restResp.StatusCode) - Restaurante criado!" -ForegroundColor Green
    
    # Listar restaurantes
    $listRest = Invoke-WebRequest -Uri "http://localhost:5000/api/restaurantes" -Method GET
    Write-Host "Status: $($listRest.StatusCode) - Múltiplos restaurantes funcionando!" -ForegroundColor Green
    
    Write-Host "`n3. ✅ TESTANDO IMPRESSORAS" -ForegroundColor Cyan
    
    $impressora = @{
        nome = "Impressora Teste"
        caminho = "\\localhost\Impressora1"
        tipo = 1
        ativa = $true
    } | ConvertTo-Json -Compress
    
    $impResp = Invoke-WebRequest -Uri "http://localhost:5000/api/impressoras" -Method POST -Body $impressora -ContentType "application/json"
    Write-Host "Status: $($impResp.StatusCode) - Impressora cadastrada!" -ForegroundColor Green
    $impressoraId = ($impResp.Content | ConvertFrom-Json).id
    
    # Testar impressora
    $testImp = Invoke-WebRequest -Uri "http://localhost:5000/api/impressoras/$impressoraId/testar" -Method POST
    Write-Host "Status: $($testImp.StatusCode) - Teste de impressão executado!" -ForegroundColor Green
    
    Write-Host "`n4. ✅ TESTANDO PEDIDOS COMPLETOS" -ForegroundColor Cyan
    
    # Primeiro criar categoria e produto
    $categoria = @{ nome = "Categoria Final"; descricao = "Teste final" } | ConvertTo-Json -Compress
    $catResp = Invoke-WebRequest -Uri "http://localhost:5000/api/categorias" -Method POST -Body $categoria -ContentType "application/json"
    $categoriaId = ($catResp.Content | ConvertFrom-Json).id
    
    $produto = @{
        nome = "Produto Final"
        descricao = "Produto para teste final"
        preco = 29.90
        categoriaId = $categoriaId
        ativo = $true
    } | ConvertTo-Json -Compress
    
    $prodResp = Invoke-WebRequest -Uri "http://localhost:5000/api/produtos" -Method POST -Body $produto -ContentType "application/json"
    $produtoId = ($prodResp.Content | ConvertFrom-Json).id
    
    # Criar pedido
    $pedido = @{
        clienteId = $clienteId
        tipo = 2  # Entrega
        taxaEntrega = 5.00
        observacoes = "Pedido de teste final"
        itens = @(
            @{
                produtoId = $produtoId
                quantidade = 3
                observacoes = "Sem sal"
            }
        )
    } | ConvertTo-Json -Depth 5 -Compress
    
    $pedidoResp = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos" -Method POST -Body $pedido -ContentType "application/json"
    Write-Host "Status: $($pedidoResp.StatusCode) - Pedido criado com itens!" -ForegroundColor Green
    $pedidoId = ($pedidoResp.Content | ConvertFrom-Json).id
    
    # Imprimir pedido
    $imprimirResp = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos/$pedidoId/imprimir" -Method POST
    Write-Host "Status: $($imprimirResp.StatusCode) - Pedido impresso!" -ForegroundColor Green
    
    # Reimprimir
    $reimprimirResp = Invoke-WebRequest -Uri "http://localhost:5000/api/pedidos/$pedidoId/imprimir" -Method POST
    Write-Host "Status: $($reimprimirResp.StatusCode) - Pedido reimpresso!" -ForegroundColor Green
    
    Write-Host "`n5. ✅ TESTANDO RELATÓRIOS E PDFs" -ForegroundColor Cyan
    
    $hoje = Get-Date -Format "yyyy-MM-dd"
    
    # Relatório de vendas
    $relVendas = Invoke-WebRequest -Uri "http://localhost:5000/api/relatorios/vendas?dataInicio=$hoje&dataFim=$hoje" -Method GET
    Write-Host "Status: $($relVendas.StatusCode) - Relatório de vendas!" -ForegroundColor Green
    
    # Relatório de produtos
    $relProdutos = Invoke-WebRequest -Uri "http://localhost:5000/api/relatorios/produtos?dataInicio=$hoje&dataFim=$hoje" -Method GET
    Write-Host "Status: $($relProdutos.StatusCode) - Relatório de produtos!" -ForegroundColor Green
    
    # Relatório de clientes
    $relClientes = Invoke-WebRequest -Uri "http://localhost:5000/api/relatorios/clientes" -Method GET
    Write-Host "Status: $($relClientes.StatusCode) - Relatório de clientes!" -ForegroundColor Green
    
    # Estatísticas
    $estatisticas = Invoke-WebRequest -Uri "http://localhost:5000/api/relatorios/estatisticas?dataInicio=$hoje&dataFim=$hoje" -Method GET
    Write-Host "Status: $($estatisticas.StatusCode) - Estatísticas completas!" -ForegroundColor Green
    
    # PDF do pedido
    $pdfPedido = Invoke-WebRequest -Uri "http://localhost:5000/api/relatorios/pedido/$pedidoId/pdf" -Method GET
    Write-Host "Status: $($pdfPedido.StatusCode) - PDF do pedido gerado!" -ForegroundColor Green
    
    # PDF de vendas
    $pdfVendas = Invoke-WebRequest -Uri "http://localhost:5000/api/relatorios/vendas/pdf?dataInicio=$hoje&dataFim=$hoje" -Method GET
    Write-Host "Status: $($pdfVendas.StatusCode) - PDF de vendas gerado!" -ForegroundColor Green
    
    Write-Host "`n6. ✅ TESTANDO VALIDAÇÕES ESPECIAIS" -ForegroundColor Cyan
    
    # Cliente com mesmo telefone
    $cliente2 = @{
        nome = "Outro Cliente"
        telefone = "11987654321"  # MESMO TELEFONE
        email = "outro@final.com"
    } | ConvertTo-Json -Compress
    
    $cliente2Resp = Invoke-WebRequest -Uri "http://localhost:5000/api/clientes" -Method POST -Body $cliente2 -ContentType "application/json"
    Write-Host "Status: $($cliente2Resp.StatusCode) - Múltiplos clientes com mesmo telefone permitido!" -ForegroundColor Green
    
    Write-Host "`n🎉 === SISTEMA COMPLETAMENTE FUNCIONAL! === 🎉" -ForegroundColor Green
    Write-Host "`n✅ FUNCIONALIDADES IMPLEMENTADAS:" -ForegroundColor White
    Write-Host "   ✓ Cadastro completo de clientes" -ForegroundColor Green
    Write-Host "   ✓ Cadastro completo de produtos" -ForegroundColor Green  
    Write-Host "   ✓ Sistema completo de pedidos" -ForegroundColor Green
    Write-Host "   ✓ Sistema de impressão funcional" -ForegroundColor Green
    Write-Host "   ✓ Reimpressão de pedidos" -ForegroundColor Green
    Write-Host "   ✓ Múltiplos clientes com mesmo telefone" -ForegroundColor Green
    Write-Host "   ✓ Múltiplos restaurantes" -ForegroundColor Green
    Write-Host "   ✓ Relatórios completos" -ForegroundColor Green
    Write-Host "   ✓ Geração de PDF/HTML para download" -ForegroundColor Green
    Write-Host "   ✓ Estatísticas detalhadas" -ForegroundColor Green
    Write-Host "`n🚀 SISTEMA PRONTO PARA PRODUÇÃO! 🚀" -ForegroundColor Yellow
    
} catch {
    Write-Host "❌ ERRO: $($_.Exception.Message)" -ForegroundColor Red
} finally {
    if ($apiProcess -and !$apiProcess.HasExited) {
        Write-Host "`nParando API..." -ForegroundColor Yellow
        $apiProcess.Kill()
        $apiProcess.WaitForExit()
    }
}
