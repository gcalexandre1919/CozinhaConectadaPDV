# ==============================================================================
# COZINHA CONECTADA - Executar com SQL Server
# ==============================================================================

Write-Host "🍽️ COZINHA CONECTADA - SQL SERVER MODE" -ForegroundColor Green
Write-Host "=====================================" -ForegroundColor Green
Write-Host ""

# Verificar se o SQL Server está rodando
Write-Host "🔍 Verificando SQL Server..." -ForegroundColor Yellow
$sqlService = Get-Service -Name "MSSQL*" -ErrorAction SilentlyContinue
if ($sqlService) {
    Write-Host "✅ SQL Server encontrado!" -ForegroundColor Green
} else {
    Write-Host "❌ SQL Server não encontrado. Verifique a instalação." -ForegroundColor Red
    exit 1
}

# Configurar banco de dados
Write-Host "🔧 Configurando banco de dados..." -ForegroundColor Blue
if (Test-Path ".\scripts\criar-banco-sqlserver.sql") {
    Write-Host "📊 Executando script de banco..." -ForegroundColor Cyan
    # Aqui você pode adicionar o comando para executar o SQL
}

# Executar API
Write-Host "🚀 Iniciando API..." -ForegroundColor Blue
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '.\API\SistemaPDV.API'; dotnet run"

# Aguardar API inicializar
Write-Host "⏳ Aguardando API inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 5

# Executar Web
Write-Host "🌐 Iniciando aplicação Web..." -ForegroundColor Blue
Start-Process powershell -ArgumentList "-NoExit", "-Command", "cd '.\Web\SistemaPDV.Web'; dotnet run"

Write-Host ""
Write-Host "✅ Sistema iniciado com SQL Server!" -ForegroundColor Green
Write-Host "Frontend: http://localhost:5107" -ForegroundColor Cyan
Write-Host "API: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host ""
Write-Host "Pressione qualquer tecla para sair..." -ForegroundColor Gray
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")