# 🚀 Instruções de Execução - Sistema PDV (SQL Server)

## Pré-requisitos

### 1. Instalar o .NET 8.0 SDK
```bash
# Windows
# Baixe e instale do site oficial: https://dotnet.microsoft.com/download
```

### 2. Instalar SQL Server
Escolha uma das opções:

#### Opção A: SQL Server Express LocalDB (Recomendado - Mais Simples)
```bash
# Download: https://docs.microsoft.com/pt-br/sql/database-engine/configure-windows/sql-server-express-localdb
# Ou instale o Visual Studio que já inclui LocalDB
```

#### Opção B: SQL Server Express
```bash
# Download: https://www.microsoft.com/pt-br/sql-server/sql-server-downloads
# Escolha "Express" e baixe a versão gratuita
```

#### Opção C: SQL Server Developer/Standard
```bash
# Para desenvolvimento: SQL Server Developer (gratuito)
# Download: https://www.microsoft.com/pt-br/sql-server/sql-server-downloads
```

### 3. Instalar HeidiSQL (Cliente para gerenciar banco)
```bash
# Download: https://www.heidisql.com/download.php
# Cliente visual gratuito para SQL Server
```

## 📁 Estrutura do Projeto

```
SistemaPDV/
├── src/
│   ├── Core/SistemaPDV.Core/           # Entidades e interfaces
│   ├── Infrastructure/SistemaPDV.Infrastructure/  # Repositórios e DbContext
│   ├── API/SistemaPDV.API/             # Web API (Backend)
│   └── Web/SistemaPDV.Web/             # Blazor Server (Frontend)
├── README.md
├── INSTRUCOES_EXECUCAO.md
└── todo.md
```

## ⚙️ Configuração

### 1. Executar Script Automático (Recomendado)

Para facilitar a execução, use o script PowerShell que configura tudo automaticamente:

```powershell
# Abrir PowerShell como Administrador na pasta do projeto
.\executar-sistema.ps1
```

O script irá:
- Detectar o tipo de SQL Server instalado
- Configurar a string de conexão automaticamente
- Restaurar pacotes NuGet
- Criar e aplicar migrações
- Iniciar API e Frontend
- Mostrar informações para conectar com HeidiSQL

### 2. Configuração Manual

Se preferir configurar manualmente, edite o arquivo `src/API/SistemaPDV.API/appsettings.json`:

#### Para LocalDB:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=SistemaPDV;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JWT": {
    "ValidAudience": "http://localhost:5000",
    "ValidIssuer": "http://localhost:5000",
    "Secret": "SuperSecretKey@123"
  }
}
```

#### Para SQL Server Express:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=.\\SQLEXPRESS;Database=SistemaPDV;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JWT": {
    "ValidAudience": "http://localhost:5000",
    "ValidIssuer": "http://localhost:5000",
    "Secret": "SuperSecretKey@123"
  }
}
```

#### Para SQL Server completo:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=SistemaPDV;Trusted_Connection=true;MultipleActiveResultSets=true"
  },
  "JWT": {
    "ValidAudience": "http://localhost:5000",
    "ValidIssuer": "http://localhost:5000",
    "Secret": "SuperSecretKey@123"
  }
}
```

### 3. Configurar Frontend

Edite o arquivo `src/Web/SistemaPDV.Web/appsettings.json`:

```json
{
  "ApiSettings": {
    "BaseUrl": "http://localhost:5000"
  }
}
```

## 🏃‍♂️ Executando o Sistema

### Opção 1: Script Automático (Recomendado)

```powershell
# Execute na pasta raiz do projeto
.\executar-sistema.ps1
```

### Opção 2: Executar API e Frontend Separadamente

#### Terminal 1 - API (Backend)
```powershell
cd src\API\SistemaPDV.API
dotnet restore
dotnet ef database update
dotnet run
```
A API estará disponível em: `http://localhost:5000`

#### Terminal 2 - Frontend
```powershell
cd src\Web\SistemaPDV.Web
dotnet restore
dotnet run
```
O frontend estará disponível em: `http://localhost:5107`

### Opção 3: Script PowerShell Personalizado

Crie um arquivo `executar.ps1` na raiz do projeto:

```powershell
Write-Host "🚀 Iniciando Sistema PDV..." -ForegroundColor Green

# Executar API em background
Write-Host "📡 Iniciando API..." -ForegroundColor Yellow
$apiJob = Start-Job -ScriptBlock {
    Set-Location "src\API\SistemaPDV.API"
    dotnet run --urls http://localhost:5000
}

# Aguardar API inicializar
Start-Sleep -Seconds 15

# Executar Frontend
Write-Host "🌐 Iniciando Frontend..." -ForegroundColor Yellow
$webJob = Start-Job -ScriptBlock {
    Set-Location "src\Web\SistemaPDV.Web"
    dotnet run --urls http://localhost:5107
}

Write-Host "✅ Sistema iniciado!" -ForegroundColor Green
Write-Host "📡 API: http://localhost:5000" -ForegroundColor Cyan
Write-Host "🌐 Frontend: http://localhost:5107" -ForegroundColor Cyan

# Aguardar interrupção
try {
    while ($true) { Start-Sleep -Seconds 5 }
} finally {
    Stop-Job $apiJob, $webJob
    Remove-Job $apiJob, $webJob
}
```

## 🔧 Solução de Problemas

### Problema: Erro de conexão com banco de dados
**Solução:**
1. Verifique se o SQL Server está rodando:
   ```powershell
   # Para LocalDB
   SqlLocalDB info MSSQLLocalDB
   SqlLocalDB start MSSQLLocalDB
   
   # Para SQL Server Express/Full
   Get-Service MSSQL*
   ```
2. Verifique a string de conexão no `appsettings.json`
3. Teste a conexão com HeidiSQL ou SQL Server Management Studio

### Problema: LocalDB não encontrado
**Solução:**
1. Instalar SQL Server Express LocalDB:
   ```
   Download: https://docs.microsoft.com/pt-br/sql/database-engine/configure-windows/sql-server-express-localdb
   ```
2. Ou usar SQL Server Express completo
3. Ou instalar Visual Studio (que inclui LocalDB)

### Problema: Porta já em uso
**Solução:**
1. Verificar processos usando as portas:
   ```powershell
   netstat -ano | findstr :5000
   netstat -ano | findstr :5107
   ```
2. Matar processos se necessário:
   ```powershell
   taskkill /PID NUMERO_DO_PID /F
   ```

### Problema: Erro de migração do banco
**Solução:**
1. Remover migrações antigas:
   ```powershell
   cd src\API\SistemaPDV.API
   Remove-Item Migrations -Recurse -Force
   ```
2. Criar nova migração:
   ```powershell
   dotnet ef migrations add InitialCreateSqlServer
   dotnet ef database update
   ```

### Problema: Erro de CORS
**Solução:**
- Verificar se a configuração de CORS está correta na API
- Verificar se a URL base no frontend está correta

### Problema: Erro de autenticação SQL Server
**Solução:**
1. Para Windows Authentication (recomendado):
   - Usar `Trusted_Connection=true` na string de conexão
   - Executar aplicação com usuário que tem acesso ao SQL Server
2. Para SQL Authentication:
   - Habilitar modo misto no SQL Server
   - Criar usuário SQL e usar na string de conexão

## � Conectando ao Banco com HeidiSQL

### 1. Configuração para LocalDB
1. Abrir HeidiSQL
2. Criar nova sessão:
   - **Network type**: Microsoft SQL Server (TCP/IP)
   - **Hostname / IP**: `(localdb)\mssqllocaldb`
   - **User**: *(deixar em branco)*
   - **Password**: *(deixar em branco)*
   - **Port**: 1433
3. Clicar em "Open"
4. Selecionar banco `SistemaPDV`

### 2. Configuração para SQL Server Express
1. Abrir HeidiSQL
2. Criar nova sessão:
   - **Network type**: Microsoft SQL Server (TCP/IP)
   - **Hostname / IP**: `.\SQLEXPRESS` ou `localhost\SQLEXPRESS`
   - **User**: *(deixar em branco para Windows Auth)*
   - **Password**: *(deixar em branco para Windows Auth)*
   - **Port**: 1433
3. Clicar em "Open"
4. Selecionar banco `SistemaPDV`

### 3. Configuração para SQL Server Completo
1. Abrir HeidiSQL
2. Criar nova sessão:
   - **Network type**: Microsoft SQL Server (TCP/IP)
   - **Hostname / IP**: `localhost`
   - **User**: *(deixar em branco para Windows Auth)*
   - **Password**: *(deixar em branco para Windows Auth)*
   - **Port**: 1433
3. Clicar em "Open"
4. Selecionar banco `SistemaPDV`

### 4. Tabelas Principais
Após conectar, você verá as seguintes tabelas:
- **AspNetUsers**: Usuários do sistema
- **Restaurantes**: Dados dos restaurantes
- **Produtos**: Cardápio/produtos
- **Clientes**: Cadastro de clientes
- **Enderecos**: Endereços dos clientes
- **Pedidos**: Pedidos realizados
- **ItensPedido**: Itens de cada pedido

## 📱 Primeiro Acesso

### 1. Registrar um Restaurante
1. Acesse: `http://localhost:5001/register`
2. Preencha os dados do restaurante
3. Clique em "Registrar"

### 2. Fazer Login
1. Acesse: `http://localhost:5001/login`
2. Use o email e senha cadastrados
3. Clique em "Entrar"

### 3. Navegar pelo Sistema
- **Dashboard**: Visão geral
- **Produtos**: Cadastrar pratos, bebidas, etc.
- **Clientes**: Cadastrar clientes
- **Pedidos**: Criar novos pedidos
- **Relatórios**: Análises e estatísticas

## 🎯 Funcionalidades Principais

### Cadastro de Produtos
1. Ir em "Produtos" → "Novo Produto"
2. Preencher nome, categoria, preço
3. Salvar

### Cadastro de Clientes
1. Ir em "Clientes" → "Novo Cliente"
2. Preencher nome e telefone (obrigatórios)
3. Adicionar endereços se necessário
4. Salvar

### Criar Pedido
1. Ir em "Pedidos" → "Novo Pedido"
2. Selecionar cliente
3. Escolher tipo de pedido:
   - **Retirada**: Cliente busca
   - **Entrega**: Delivery (com taxa)
   - **Refeição no Local**: Com taxa de serviço
4. Adicionar produtos
5. Salvar pedido

### Fechar Conta (Refeição no Local)
1. Ir em "Pedidos"
2. Localizar pedido aberto
3. Clicar em "Fechar Conta"

## 📊 Relatórios

### Vendas por Período
- Selecionar datas de início e fim
- Visualizar total de pedidos e faturamento

### Produtos Mais Vendidos
- Ver ranking de produtos
- Filtrar por período

### Histórico do Cliente
- Inserir ID do cliente
- Ver histórico completo de pedidos

## 🔒 Segurança

- Sistema usa JWT para autenticação
- Cada restaurante tem dados isolados
- Senhas são criptografadas
- Validações no frontend e backend

## 📞 Suporte

Em caso de problemas:
1. Verificar logs no terminal
2. Verificar configurações de banco
3. Verificar se todas as dependências estão instaladas
4. Reiniciar os serviços

---

**Sistema PDV - Desenvolvido com C# e ASP.NET Core + SQL Server** 🚀

