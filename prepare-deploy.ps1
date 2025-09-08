# Script PowerShell para fazer upload dos arquivos de deploy
# Execute este script no Windows antes de fazer o deploy

Write-Host "Preparando arquivos para deploy Oracle Cloud..." -ForegroundColor Green

# Verificar se Git esta configurado
$gitStatus = git status 2>$null
if ($LASTEXITCODE -ne 0) {
    Write-Host "Git nao encontrado. Instale o Git primeiro." -ForegroundColor Red
    exit 1
}

# Adicionar novos arquivos
Write-Host "Adicionando arquivos de deploy..." -ForegroundColor Yellow
git add dockerfile
git add docker-compose.yml
git add deploy.sh
git add DEPLOY_ORACLE_CLOUD.md
git add Web/SistemaPDV.Web/appsettings.Production.json
git add prepare-deploy.ps1

# Commit
Write-Host "Fazendo commit..." -ForegroundColor Yellow
git commit -m "Adicionar configurações de deploy para Oracle Cloud Always Free

- Dockerfile otimizado para Oracle Cloud
- Docker Compose para produção
- Script de deploy automatizado
- Documentação completa do processo
- Configurações de produção"

# Push
Write-Host "Enviando para GitHub..." -ForegroundColor Yellow
git push origin main

Write-Host ""
Write-Host "Arquivos enviados para GitHub!" -ForegroundColor Green
Write-Host ""
Write-Host "Proximos passos:" -ForegroundColor Cyan
Write-Host "1. Criar conta no Oracle Cloud Always Free" -ForegroundColor White
Write-Host "2. Criar VM Ubuntu 22.04" -ForegroundColor White
Write-Host "3. Configurar Security Lists" -ForegroundColor White
Write-Host "4. Seguir o guia em DEPLOY_ORACLE_CLOUD.md" -ForegroundColor White
Write-Host ""
Write-Host "Documentacao completa em: DEPLOY_ORACLE_CLOUD.md" -ForegroundColor Magenta

# Pausa para o usuario ler
Read-Host "Pressione Enter para continuar..."
