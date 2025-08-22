# Script simples para executar o Sistema PDV

Write-Host "Iniciando Sistema PDV com SQL Server..." -ForegroundColor Green

# Navegar para o diretorio da API
Write-Host "Iniciando API..." -ForegroundColor Yellow
cd "src\API\SistemaPDV.API"

# Restaurar e executar API em background
dotnet restore
$apiJob = Start-Job -ScriptBlock {
    Set-Location "c:\Users\GC\Documents\GitHub\CozinhaConectada\SistemaPDV\src\API\SistemaPDV.API"
    dotnet run --urls http://localhost:5000
}

Start-Sleep -Seconds 10

# Navegar para o Frontend
cd "..\..\Web\SistemaPDV.Web"
Write-Host "Iniciando Frontend..." -ForegroundColor Yellow

# Restaurar e executar Frontend em background
dotnet restore
$webJob = Start-Job -ScriptBlock {
    Set-Location "c:\Users\GC\Documents\GitHub\CozinhaConectada\SistemaPDV\src\Web\SistemaPDV.Web"
    dotnet run --urls http://localhost:5107
}

Start-Sleep -Seconds 10

Write-Host ""
Write-Host "Sistema iniciado!" -ForegroundColor Green
Write-Host "API: http://localhost:5000" -ForegroundColor Cyan
Write-Host "Swagger: http://localhost:5000/swagger" -ForegroundColor Cyan
Write-Host "Frontend: http://localhost:5107" -ForegroundColor Cyan
Write-Host ""
Write-Host "Para conectar ao banco SQL Server com HeidiSQL:" -ForegroundColor Yellow
Write-Host "Servidor: .\SQLEXPRESS" -ForegroundColor White
Write-Host "Banco: SistemaPDV" -ForegroundColor White
Write-Host "Autenticacao: Windows Authentication" -ForegroundColor White
Write-Host ""
Write-Host "Pressione Ctrl+C para parar" -ForegroundColor Yellow

# Aguardar interrupcao
try {
    while ($true) {
        Start-Sleep -Seconds 5
    }
} finally {
    Write-Host "Parando servicos..." -ForegroundColor Yellow
    Stop-Job $apiJob, $webJob -ErrorAction SilentlyContinue
    Remove-Job $apiJob, $webJob -ErrorAction SilentlyContinue
    Write-Host "Servicos parados" -ForegroundColor Green
}
