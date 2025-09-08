# Script para deploy no Oracle Cloud Always Free
#!/bin/bash

echo "🚀 Iniciando deploy no Oracle Cloud Always Free..."

# Parar containers existentes
echo "📦 Parando containers existentes..."
sudo docker-compose down

# Fazer backup do banco de dados
echo "💾 Fazendo backup do banco..."
if [ -f "./data/SistemaPDV.db" ]; then
    cp ./data/SistemaPDV.db "./data/SistemaPDV.db.backup.$(date +%Y%m%d_%H%M%S)"
fi

# Atualizar código do Git
echo "🔄 Atualizando código..."
git pull origin main

# Rebuild e restart
echo "🔨 Rebuilding aplicação..."
sudo docker-compose build --no-cache

echo "🚀 Iniciando aplicação..."
sudo docker-compose up -d

# Verificar status
echo "✅ Verificando status..."
sudo docker-compose ps

# Mostrar logs
echo "📋 Logs da aplicação:"
sudo docker-compose logs --tail=50 app

echo ""
echo "🎉 Deploy concluído!"
echo "🌐 Aplicação disponível em: http://$(curl -s ifconfig.me):8080"
echo "📊 Para ver logs: sudo docker-compose logs -f app"
echo "🔄 Para restart: sudo docker-compose restart"
