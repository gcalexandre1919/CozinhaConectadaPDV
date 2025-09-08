# README - Deploy Oracle Cloud Always Free

## 🚀 Como hospedar no Oracle Cloud Always Free

### 💰 **Garantia de Custo Zero**
- Oracle Cloud Always Free oferece recursos 100% gratuitos permanentemente
- Inclui: 2 VMs AMD (1/8 OCPU, 1GB RAM cada) ou 1 VM ARM (4 OCPUs, 24GB RAM)
- Não há cobrança automática mesmo se você exceder (precisa converter manualmente)

### 📋 **Passo a Passo Completo**

#### 1️⃣ **Criar Conta Oracle Cloud**
1. Acesse: https://www.oracle.com/cloud/free/
2. Clique em "Start for free"
3. Preencha dados (não precisará de cartão de crédito no Always Free)
4. Verifique email e ative a conta

#### 2️⃣ **Criar Instância VM**
1. No painel Oracle Cloud, vá em "Compute" > "Instances"
2. Clique "Create Instance"
3. **Configurações recomendadas:**
   - **Name:** pdv-sistema
   - **Image:** Ubuntu 22.04 (Always Free)
   - **Shape:** VM.Standard.E2.1.Micro (Always Free)
   - **Boot Volume:** 50GB (Always Free)
4. **SSH Keys:** Gere ou upload sua chave SSH
5. **Networking:** Deixe padrão (cria VCN automaticamente)
6. Clique "Create"

#### 3️⃣ **Configurar Rede (IMPORTANTE)**
1. Vá em "Networking" > "Virtual Cloud Networks"
2. Clique na VCN criada
3. Clique na "Public Subnet"
4. Clique na "Default Security List"
5. Clique "Add Ingress Rules"
6. **Adicione estas regras:**
   ```
   Source Type: CIDR
   Source CIDR: 0.0.0.0/0
   IP Protocol: TCP
   Destination Port Range: 8080
   Description: PDV System
   ```
   ```
   Source Type: CIDR
   Source CIDR: 0.0.0.0/0
   IP Protocol: TCP
   Destination Port Range: 80
   Description: HTTP
   ```
   ```
   Source Type: CIDR
   Source CIDR: 0.0.0.0/0
   IP Protocol: TCP
   Destination Port Range: 443
   Description: HTTPS
   ```

#### 4️⃣ **Conectar na VM**
```bash
# No seu computador local
ssh -i caminho/para/sua/chave.pem ubuntu@IP_PUBLICO_DA_VM
```

#### 5️⃣ **Configurar a VM**
```bash
# Atualizar sistema
sudo apt update && sudo apt upgrade -y

# Instalar Docker
curl -fsSL https://get.docker.com -o get-docker.sh
sudo sh get-docker.sh
sudo usermod -aG docker ubuntu

# Instalar Docker Compose
sudo apt install docker-compose -y

# Reiniciar para aplicar permissões
sudo reboot
```

#### 6️⃣ **Deploy da Aplicação**
```bash
# Conectar novamente após reboot
ssh -i caminho/para/sua/chave.pem ubuntu@IP_PUBLICO_DA_VM

# Clonar projeto
git clone https://github.com/gcalexandre1919/CozinhaConectadaPDV.git
cd CozinhaConectadaPDV

# Criar diretório para dados
mkdir -p data

# Dar permissão ao script
chmod +x deploy.sh

# Executar deploy
./deploy.sh
```

#### 7️⃣ **Configurar Firewall da VM**
```bash
# Abrir portas no Ubuntu
sudo ufw enable
sudo ufw allow 22/tcp
sudo ufw allow 8080/tcp
sudo ufw allow 80/tcp
sudo ufw allow 443/tcp
```

### 🔄 **Comandos Úteis**

```bash
# Ver status
docker-compose ps

# Ver logs
docker-compose logs -f app

# Restart aplicação
docker-compose restart

# Parar aplicação
docker-compose down

# Atualizar aplicação
git pull origin main
docker-compose build --no-cache
docker-compose up -d

# Backup do banco
cp ./data/SistemaPDV.db ./data/backup_$(date +%Y%m%d).db
```

### 🌐 **Acessar a Aplicação**
- **URL:** http://IP_PUBLICO_DA_VM:8080
- **Login padrão:** admin@sistema.com / Admin123!

### 📊 **Monitoramento de Recursos**
```bash
# Ver uso de recursos
docker stats

# Ver uso do sistema
htop

# Ver espaço em disco
df -h
```

### 🔒 **Garantias de Segurança Oracle Cloud Always Free**

#### ✅ **Não há risco de cobrança porque:**
1. **Always Free é permanente** - não expira
2. **Recursos limitados** - não pode exceder automaticamente
3. **Sem cartão obrigatório** - conta Always Free não exige
4. **Conversão manual** - só vira pago se você solicitar explicitamente

#### 🛡️ **Proteções ativas:**
- Resources shapes limitados aos Always Free
- Billing desabilitado por padrão
- Alertas antes de qualquer upgrade
- Não há auto-scaling que possa gerar custo

### 📈 **Limites Always Free (Garantidos Gratuitos)**
- **Compute:** 2 VMs ou 1 ARM (3000 horas/mês)
- **Storage:** 200GB total
- **Network:** 10TB outbound/mês
- **Database:** 2 Autonomous DBs (20GB cada)

### 🆘 **Solução de Problemas**

#### Aplicação não inicia:
```bash
docker-compose logs app
```

#### Sem conexão:
- Verificar Security Lists no Oracle Cloud
- Verificar firewall: `sudo ufw status`
- Verificar se porta 8080 está aberta

#### Banco de dados:
```bash
# Verificar se arquivo existe
ls -la ./data/

# Recriar banco se necessário
docker-compose exec app dotnet ef database update
```

### 📱 **Próximos Passos (Opcionais)**
1. **Domínio próprio:** Configure DNS para seu IP
2. **HTTPS:** Configure Let's Encrypt
3. **Backup automático:** Configure cron jobs
4. **Monitoramento:** Configure alertas

---

**💡 Dica:** Mantenha seu IP público da VM anotado e configure um domínio gratuito (como .tk ou .ml) para facilitar o acesso!
