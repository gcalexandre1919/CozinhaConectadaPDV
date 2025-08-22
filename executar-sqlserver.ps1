#!/usr/bin/env pwsh

# Script para executar o Sistema PDV com SQL Server LocalDB

Write-Host "🚀 Iniciando Sistema PDV com SQL Server..." -ForegroundColor Green

# Verificar se o LocalDB está instalado
Write-Host "📋 Verificando LocalDB..." -ForegroundColor Yellow
try {
    $localDbInfo = SqlLocalDB info MSSQLLocalDB 2>$null
    if ($LASTEXITCODE -eq 0) {
        Write-Host "✅ LocalDB encontrado e funcionando" -ForegroundColor Green
    } else {
        Write-Host "❌ LocalDB não está funcionando. Tentando iniciar..." -ForegroundColor Red
        SqlLocalDB start MSSQLLocalDB
        if ($LASTEXITCODE -eq 0) {
            Write-Host "✅ LocalDB iniciado com sucesso" -ForegroundColor Green
        } else {
            Write-Host "❌ Erro ao iniciar LocalDB. Verifique a instalação do SQL Server Express LocalDB" -ForegroundColor Red
            exit 1
        }
    }
} catch {
    Write-Host "❌ LocalDB não está instalado. Por favor, instale o SQL Server Express LocalDB" -ForegroundColor Red
    Write-Host "Download: https://docs.microsoft.com/pt-br/sql/database-engine/configure-windows/sql-server-express-localdb" -ForegroundColor Yellow
    exit 1
}

# Navegar para o diretório da API
Set-Location "src\API\SistemaPDV.API"

Write-Host "📦 Restaurando pacotes NuGet..." -ForegroundColor Yellow
dotnet restore

Write-Host "🗃️ Aplicando migrações do banco de dados..." -ForegroundColor Yellow
dotnet ef database update

if ($LASTEXITCODE -ne 0) {
    Write-Host "❌ Erro ao aplicar migrações. Tentando criar migração inicial..." -ForegroundColor Red
    dotnet ef migrations add InitialCreate
    dotnet ef database update
    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Erro ao criar/aplicar migrações" -ForegroundColor Red
        exit 1
    }
}

Write-Host "✅ Banco de dados configurado com sucesso!" -ForegroundColor Green

# Iniciar API em background
Write-Host "📡 Iniciando API..." -ForegroundColor Yellow
$apiJob = Start-Job -ScriptBlock {
    Set-Location $using:PWD
    dotnet run --urls http://localhost:5000
}

# Aguardar a API inicializar
Write-Host "⏳ Aguardando API inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Verificar se a API está rodando
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger" -Method Get -UseBasicParsing -ErrorAction Stop
    Write-Host "✅ API iniciada com sucesso!" -ForegroundColor Green
    Write-Host "📡 API disponível em: http://localhost:5000" -ForegroundColor Cyan
    Write-Host "📚 Swagger UI: http://localhost:5000/swagger" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Erro ao verificar se a API está rodando" -ForegroundColor Red
}

# Navegar para o diretório do Frontend
Set-Location "..\..\Web\SistemaPDV.Web"

Write-Host "📦 Restaurando pacotes do Frontend..." -ForegroundColor Yellow
dotnet restore

# Iniciar Frontend
Write-Host "🌐 Iniciando Frontend..." -ForegroundColor Yellow
$webJob = Start-Job -ScriptBlock {
    Set-Location $using:PWD
    dotnet run --urls http://localhost:5107
}

# Aguardar o Frontend inicializar
Write-Host "⏳ Aguardando Frontend inicializar..." -ForegroundColor Yellow
Start-Sleep -Seconds 10

# Verificar se o Frontend está rodando
try {
    $response = Invoke-WebRequest -Uri "http://localhost:5107" -Method Get -UseBasicParsing -ErrorAction Stop
    Write-Host "✅ Frontend iniciado com sucesso!" -ForegroundColor Green
    Write-Host "🌐 Frontend disponível em: http://localhost:5107" -ForegroundColor Cyan
} catch {
    Write-Host "❌ Erro ao verificar se o Frontend está rodando" -ForegroundColor Red
}

Write-Host ""
Write-Host "🎉 Sistema PDV iniciado com sucesso!" -ForegroundColor Green
Write-Host "📡 API: http://localhost:5000" -ForegroundColor Cyan
Write-Host "📚 Swagger: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "🌐 Frontend: http://localhost:5107" -ForegroundColor Cyan
Write-Host ""
Write-Host "💡 Para conectar ao banco com HeidiSQL:" -ForegroundColor Yellow
Write-Host "   Servidor: (localdb)\mssqllocaldb" -ForegroundColor White
Write-Host "   Banco: SistemaPDV" -ForegroundColor White
Write-Host "   Autenticação: Windows Authentication" -ForegroundColor White
Write-Host ""
Write-Host "⚠️  Pressione Ctrl+C para parar os serviços" -ForegroundColor Yellow

# Aguardar interrupção
try {
    while ($true) {
        Start-Sleep -Seconds 5
        
        # Verificar se os jobs ainda estão rodando
        if ($apiJob.State -eq "Failed") {
            Write-Host "❌ API parou de funcionar" -ForegroundColor Red
            break
        }
        if ($webJob.State -eq "Failed") {
            Write-Host "❌ Frontend parou de funcionar" -ForegroundColor Red
            break
        }
    }
} finally {
    Write-Host ""
    Write-Host "🛑 Parando serviços..." -ForegroundColor Yellow
    
    # Parar jobs
    Stop-Job $apiJob -ErrorAction SilentlyContinue
    Stop-Job $webJob -ErrorAction SilentlyContinue
    Remove-Job $apiJob -ErrorAction SilentlyContinue
    Remove-Job $webJob -ErrorAction SilentlyContinue
    
    Write-Host "✅ Serviços parados" -ForegroundColor Green
}
