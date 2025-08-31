# ==============================================================================
# SCRIPT PARA TESTAR CADASTRO DE CLIENTES
# ==============================================================================

Write-Host "🧪 TESTANDO CADASTRO DE CLIENTES" -ForegroundColor Green
Write-Host "================================" -ForegroundColor Green
Write-Host ""

# Verificar se estamos na pasta correta
if (!(Test-Path ".\API\SistemaPDV.API") -or !(Test-Path ".\Web\SistemaPDV.Web")) {
    Write-Host "❌ Erro: Execute este script na pasta raiz do projeto!" -ForegroundColor Red
    exit 1
}

Write-Host "🏗️ Compilando projetos..." -ForegroundColor Blue

# Compilar API
Write-Host "   📦 Compilando API..." -ForegroundColor Cyan
Set-Location ".\API\SistemaPDV.API"
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro ao compilar API!" -ForegroundColor Red
    Set-Location "..\..\"
    exit 1
}
Set-Location "..\..\"

# Compilar Web
Write-Host "   🌐 Compilando Web..." -ForegroundColor Cyan
Set-Location ".\Web\SistemaPDV.Web"
dotnet build
if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro ao compilar Web!" -ForegroundColor Red
    Set-Location "..\..\"
    exit 1
}
Set-Location "..\..\"

Write-Host "✅ Compilação concluída com sucesso!" -ForegroundColor Green
Write-Host ""

Write-Host "🚀 Iniciando aplicações..." -ForegroundColor Blue

# Iniciar API em processo separado
Write-Host "   📡 Iniciando API na porta 5001..." -ForegroundColor Cyan
$apiProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '.\API\SistemaPDV.API'; dotnet run --urls=https://localhost:5001" -PassThru

# Aguardar a API inicializar
Write-Host "   ⏳ Aguardando API inicializar (10 segundos)..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Iniciar Web em processo separado
Write-Host "   🌐 Iniciando Web na porta 5000..." -ForegroundColor Cyan
$webProcess = Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '.\Web\SistemaPDV.Web'; dotnet run --urls=https://localhost:5000" -PassThru

Write-Host ""
Write-Host "✅ APLICAÇÕES INICIADAS!" -ForegroundColor Green
Write-Host "========================" -ForegroundColor Green
Write-Host ""
Write-Host "🌐 Frontend (Web):    https://localhost:5000" -ForegroundColor Cyan
Write-Host "📡 Backend (API):     https://localhost:5001" -ForegroundColor Cyan
Write-Host "📚 Swagger (API):     https://localhost:5001/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "🧪 TESTE DO CADASTRO DE CLIENTES:" -ForegroundColor Yellow
Write-Host "1. Acesse: https://localhost:5000/clientes" -ForegroundColor White
Write-Host "2. Clique em 'Novo Cliente'" -ForegroundColor White
Write-Host "3. Preencha os campos e teste todas as funcionalidades:" -ForegroundColor White
Write-Host "   - Validação de campos obrigatórios" -ForegroundColor Gray
Write-Host "   - Validação de email duplicado" -ForegroundColor Gray
Write-Host "   - Validação de CPF/CNPJ duplicado" -ForegroundColor Gray
Write-Host "   - Salvamento de dados completos" -ForegroundColor Gray
Write-Host "   - Listagem de clientes" -ForegroundColor Gray
Write-Host "   - Busca por clientes" -ForegroundColor Gray
Write-Host "   - Edição de clientes" -ForegroundColor Gray
Write-Host "   - Exclusão de clientes" -ForegroundColor Gray
Write-Host ""
Write-Host "🛑 Para parar as aplicações:" -ForegroundColor Red
Write-Host "   - Feche as janelas do PowerShell que abriram" -ForegroundColor Gray
Write-Host "   - Ou pressione Ctrl+C nas janelas dos terminais" -ForegroundColor Gray
Write-Host ""

# Aguardar Web inicializar
Write-Host "⏳ Aguardando Web inicializar (15 segundos)..." -ForegroundColor Yellow
Start-Sleep -Seconds 15

# Abrir navegador automaticamente
try {
    Write-Host "🌐 Abrindo navegador..." -ForegroundColor Blue
    Start-Process "https://localhost:5000/clientes"
} catch {
    Write-Host "ℹ️ Não foi possível abrir o navegador automaticamente." -ForegroundColor Yellow
    Write-Host "   Acesse manualmente: https://localhost:5000/clientes" -ForegroundColor White
}

Write-Host ""
Write-Host "Pressione qualquer tecla para sair..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")

# Finalizar processos se ainda estiverem rodando
try {
    if (!$apiProcess.HasExited) {
        Stop-Process -Id $apiProcess.Id -Force
    }
    if (!$webProcess.HasExited) {
        Stop-Process -Id $webProcess.Id -Force
    }
} catch {
    # Ignorar erros ao finalizar processos
}
