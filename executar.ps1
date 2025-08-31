Write-Host "=== EXECUTANDO SISTEMA PDV ===" -ForegroundColor Green

Write-Host "`nCompilando projeto..." -ForegroundColor Yellow
dotnet build API\SistemaPDV.API\SistemaPDV.API.csproj

Write-Host "`nIniciando API..." -ForegroundColor Green
Write-Host "Acesse: http://localhost:5000" -ForegroundColor Cyan

dotnet run --project API\SistemaPDV.API\SistemaPDV.API.csproj
