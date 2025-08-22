#!/usr/bin/env pwsh

# Script para executar o Sistema PDV com SQL Server Express

Write-Host "🚀 Iniciando Sistema PDV com SQL Server Express..." -ForegroundColor Green

# Opções de conexão
Write-Host ""
Write-Host "📋 Escolha o tipo de conexão SQL Server:" -ForegroundColor Yellow
Write-Host "1. LocalDB (padrão - mais simples)" -ForegroundColor White
Write-Host "2. SQL Server Express local" -ForegroundColor White
Write-Host "3. SQL Server completo local" -ForegroundColor White
Write-Host ""

$choice = Read-Host "Digite sua escolha (1-3) [1]"
if ([string]::IsNullOrEmpty($choice)) {
    $choice = "1"
}

# Configurar string de conexão baseada na escolha
switch ($choice) {
    "1" {
        $connectionString = "Server=(localdb)\mssqllocaldb;Database=SistemaPDV;Trusted_Connection=true;MultipleActiveResultSets=true"
        Write-Host "✅ Usando LocalDB" -ForegroundColor Green
    }
    "2" {
        $connectionString = "Server=.\SQLEXPRESS;Database=SistemaPDV;Trusted_Connection=true;MultipleActiveResultSets=true"
        Write-Host "✅ Usando SQL Server Express" -ForegroundColor Green
    }
    "3" {
        $connectionString = "Server=localhost;Database=SistemaPDV;Trusted_Connection=true;MultipleActiveResultSets=true"
        Write-Host "✅ Usando SQL Server completo" -ForegroundColor Green
    }
    default {
        $connectionString = "Server=(localdb)\mssqllocaldb;Database=SistemaPDV;Trusted_Connection=true;MultipleActiveResultSets=true"
        Write-Host "✅ Usando LocalDB (padrão)" -ForegroundColor Green
    }
}

# Atualizar appsettings.json temporariamente
$apiConfigPath = "src\API\SistemaPDV.API\appsettings.json"
$apiDevConfigPath = "src\API\SistemaPDV.API\appsettings.Development.json"

# Backup dos arquivos originais
Copy-Item $apiConfigPath "$apiConfigPath.backup" -Force
Copy-Item $apiDevConfigPath "$apiDevConfigPath.backup" -Force

try {
    # Atualizar configuração
    $config = Get-Content $apiConfigPath | ConvertFrom-Json
    $config.ConnectionStrings.DefaultConnection = $connectionString
    $config | ConvertTo-Json -Depth 10 | Set-Content $apiConfigPath

    $devConfig = Get-Content $apiDevConfigPath | ConvertFrom-Json
    $devConfig.ConnectionStrings.DefaultConnection = $connectionString
    $devConfig | ConvertTo-Json -Depth 10 | Set-Content $apiDevConfigPath

    Write-Host "📋 Configuração atualizada" -ForegroundColor Green

    # Navegar para o diretório da API
    Set-Location "src\API\SistemaPDV.API"

    Write-Host "📦 Restaurando pacotes NuGet..." -ForegroundColor Yellow
    dotnet restore

    Write-Host "🗃️ Removendo migrações antigas..." -ForegroundColor Yellow
    if (Test-Path "Migrations") {
        Remove-Item "Migrations" -Recurse -Force
    }

    Write-Host "🗃️ Criando nova migração para SQL Server..." -ForegroundColor Yellow
    dotnet ef migrations add InitialCreateSqlServer

    Write-Host "🗃️ Aplicando migrações do banco de dados..." -ForegroundColor Yellow
    dotnet ef database update

    if ($LASTEXITCODE -ne 0) {
        Write-Host "❌ Erro ao aplicar migrações" -ForegroundColor Red
        exit 1
    }

    Write-Host "✅ Banco de dados SQL Server configurado com sucesso!" -ForegroundColor Green

    # Iniciar API em background
    Write-Host "📡 Iniciando API..." -ForegroundColor Yellow
    $apiJob = Start-Job -ScriptBlock {
        Set-Location $using:PWD
        dotnet run --urls http://localhost:5000
    }

    # Aguardar a API inicializar
    Write-Host "⏳ Aguardando API inicializar..." -ForegroundColor Yellow
    Start-Sleep -Seconds 15

    # Verificar se a API está rodando
    $apiRunning = $false
    for ($i = 0; $i -lt 5; $i++) {
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5000/swagger" -Method Get -UseBasicParsing -ErrorAction Stop -TimeoutSec 5
            Write-Host "✅ API iniciada com sucesso!" -ForegroundColor Green
            $apiRunning = $true
            break
        } catch {
            Write-Host "⏳ Aguardando API... (tentativa $($i+1)/5)" -ForegroundColor Yellow
            Start-Sleep -Seconds 5
        }
    }

    if (-not $apiRunning) {
        Write-Host "❌ Timeout ao verificar se a API está rodando" -ForegroundColor Red
        Write-Host "📋 Verificando logs da API..." -ForegroundColor Yellow
        Receive-Job $apiJob
    } else {
        Write-Host "📡 API disponível em: http://localhost:5000" -ForegroundColor Cyan
        Write-Host "📚 Swagger UI: http://localhost:5000/swagger" -ForegroundColor Cyan
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
    Start-Sleep -Seconds 15

    # Verificar se o Frontend está rodando
    $webRunning = $false
    for ($i = 0; $i -lt 5; $i++) {
        try {
            $response = Invoke-WebRequest -Uri "http://localhost:5107" -Method Get -UseBasicParsing -ErrorAction Stop -TimeoutSec 5
            Write-Host "✅ Frontend iniciado com sucesso!" -ForegroundColor Green
            $webRunning = $true
            break
        } catch {
            Write-Host "⏳ Aguardando Frontend... (tentativa $($i+1)/5)" -ForegroundColor Yellow
            Start-Sleep -Seconds 5
        }
    }

    if (-not $webRunning) {
        Write-Host "❌ Timeout ao verificar se o Frontend está rodando" -ForegroundColor Red
        Write-Host "📋 Verificando logs do Frontend..." -ForegroundColor Yellow
        Receive-Job $webJob
    } else {
        Write-Host "🌐 Frontend disponível em: http://localhost:5107" -ForegroundColor Cyan
    }

    Write-Host ""
    Write-Host "🎉 Sistema PDV iniciado com sucesso!" -ForegroundColor Green
    Write-Host "📡 API: http://localhost:5000" -ForegroundColor Cyan
    Write-Host "📚 Swagger: http://localhost:5000/swagger" -ForegroundColor Cyan
    Write-Host "🌐 Frontend: http://localhost:5107" -ForegroundColor Cyan
    Write-Host ""
    Write-Host "💡 Para conectar ao banco com HeidiSQL:" -ForegroundColor Yellow
    
    switch ($choice) {
        "1" {
            Write-Host "   Tipo: Microsoft SQL Server (TCP/IP)" -ForegroundColor White
            Write-Host "   Hostname: (localdb)\mssqllocaldb" -ForegroundColor White
            Write-Host "   Usuário: (deixe em branco para Windows Auth)" -ForegroundColor White
            Write-Host "   Senha: (deixe em branco para Windows Auth)" -ForegroundColor White
        }
        "2" {
            Write-Host "   Tipo: Microsoft SQL Server (TCP/IP)" -ForegroundColor White
            Write-Host "   Hostname: .\SQLEXPRESS" -ForegroundColor White
            Write-Host "   Usuário: (deixe em branco para Windows Auth)" -ForegroundColor White
            Write-Host "   Senha: (deixe em branco para Windows Auth)" -ForegroundColor White
        }
        "3" {
            Write-Host "   Tipo: Microsoft SQL Server (TCP/IP)" -ForegroundColor White
            Write-Host "   Hostname: localhost" -ForegroundColor White
            Write-Host "   Usuário: (deixe em branco para Windows Auth)" -ForegroundColor White
            Write-Host "   Senha: (deixe em branco para Windows Auth)" -ForegroundColor White
        }
    }
    
    Write-Host "   Banco: SistemaPDV" -ForegroundColor White
    Write-Host ""
    Write-Host "⚠️  Pressione Ctrl+C para parar os serviços" -ForegroundColor Yellow

    # Aguardar interrupção
    try {
        while ($true) {
            Start-Sleep -Seconds 5
            
            # Verificar se os jobs ainda estão rodando
            if ($apiJob.State -eq "Failed") {
                Write-Host "❌ API parou de funcionar" -ForegroundColor Red
                Write-Host "📋 Logs da API:" -ForegroundColor Yellow
                Receive-Job $apiJob
                break
            }
            if ($webJob.State -eq "Failed") {
                Write-Host "❌ Frontend parou de funcionar" -ForegroundColor Red
                Write-Host "📋 Logs do Frontend:" -ForegroundColor Yellow
                Receive-Job $webJob
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

} finally {
    # Restaurar arquivos de configuração originais
    Write-Host "🔄 Restaurando configurações originais..." -ForegroundColor Yellow
    Set-Location "..\..\..\"
    
    if (Test-Path "$apiConfigPath.backup") {
        Move-Item "$apiConfigPath.backup" $apiConfigPath -Force
    }
    if (Test-Path "$apiDevConfigPath.backup") {
        Move-Item "$apiDevConfigPath.backup" $apiDevConfigPath -Force
    }
    
    Write-Host "✅ Configurações restauradas" -ForegroundColor Green
}
