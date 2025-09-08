# Sistema PDV - Cozinha Conectada

![.NET 8](https://img.shields.io/badge/.NET-8.0-512BD4?style=for-the-badg## 🛠️ Comandos

### Execução
```powershell
# ✅ Setup completo automatizado (FUNCIONA!)
.\setup-sistema.ps1

# Sistema completo (manual)
.\start-pdv.ps1

# Apenas API
.\start-pdv.ps1 -ApiOnly

# Apenas Web  
.\start-pdv.ps1 -WebOnly

# Build da solution
dotnet build SistemaPDV.sln
```

### Setup Inicial
```powershell
# ✅ MÉTODO 1: Setup automático (RECOMENDADO - FUNCIONA!)
.\setup-sistema.ps1

# MÉTODO 2: Setup manual
.\start-pdv.ps1
# Depois acesse: http://localhost:5001/swagger
# Execute: POST /api/seed/criar-dados-iniciais
# Login: admin@sistema.com / 123456
```net)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-512BD4?style=for-the-badge&logo=microsoft)
![SQLite](https://img.shields.io/badge/SQLite-003B57?style=for-the-badge&logo=sqlite)
![Status](https://img.shields.io/badge/Status-Enterprise%20Ready-00D26A?style=for-the-badge)

**Sistema PDV enterprise para restaurantes com arquitetura clean e segurança moderna**

## 🎯 Visão Geral

Sistema de Ponto de Venda (PDV) enterprise desenvolvido com .NET 8, seguindo princípios de Clean Architecture e implementando segurança de nível corporativo.

### ✨ Principais Funcionalidades

- 🔐 **Autenticação Enterprise** - JWT + Refresh Tokens, BCrypt, Account Lockout
- 👥 **Gestão de Clientes** - CRUD completo com service layer enterprise
- 🏪 **Multi-restaurantes** - Gestão de múltiplas unidades
- 🍽️ **Catálogo de Produtos** - Organização por categorias
- 📋 **Sistema de Pedidos** - Workflow otimizado cliente → pedido
- 🖨️ **Impressão Multi-área** - Impressão automática por categoria
- 📊 **Relatórios Avançados** - Analytics e business intelligence
- 🏗️ **Clean Architecture** - SOLID, DDD, Enterprise patterns

## 🚀 Início Rápido

### Pré-requisitos
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)

### Execução Simples

```powershell
# 1. Clone o repositório
git clone https://github.com/gcalexandre1919/CozinhaConectadaPDV.git
cd CozinhaConectadaPDV

# 2. Execute o setup completo (recomendado)
.\setup-sistema.ps1

# OU execute o método manual:
# .\start-pdv.ps1
# Acesse http://localhost:5001/swagger
# Execute POST /api/seed/criar-dados-iniciais
```

### Primeiro Login

**IMPORTANTE:** Após executar o setup, faça o primeiro login:

1. **Execute o setup:** `.\setup-sistema.ps1`
2. **Acesse:** http://localhost:5000
3. **Faça LOGIN com as credenciais criadas:**
   ```
   Email: admin@sistema.com
   Senha: 123456
   ```
4. **Agora todas as funcionalidades estarão disponíveis!**

### Acessos
- 🌐 **Web App**: http://localhost:5000
- 🔌 **API REST**: http://localhost:5001
- 📋 **Swagger UI**: http://localhost:5001/swagger

## 🏗️ Arquitetura Enterprise

### Estrutura Clean Architecture
```
CozinhaConectada/
├── 🔌 API/SistemaPDV.API/              # API REST Layer
├── 🎯 Core/SistemaPDV.Core/            # Domain Layer
├── 🗄️ Infrastructure/                  # Infrastructure Layer  
├── 🌐 Web/SistemaPDV.Web/              # Presentation Layer
├── � SistemaPDV.sln                   # Visual Studio Solution
└── 🚀 start-pdv.ps1                   # Startup Script
```

### Tecnologias Enterprise
- **Backend**: .NET 8, ASP.NET Core 8, Entity Framework Core 8
- **Security**: JWT, BCrypt.Net-Next, Refresh Tokens, Account Lockout
- **Database**: SQLite (development), SQL Server (production ready)
- **Architecture**: Clean Architecture, SOLID Principles, DDD

## 🔐 Recursos de Segurança

### Autenticação Moderna
- **JWT Tokens** com claims personalizados
- **Refresh Tokens** seguros (64 bytes)
- **BCrypt hashing** (cost factor 12)
- **Account lockout** após 5 tentativas falhas
- **Password strength validation** (0-100 score)
- **Login audit trail** completo

## 🎮 Funcionalidades Implementadas

### ✅ Módulos Transformados (8.5/10 Enterprise)
- [x] **Autenticação** - Sistema completo JWT + BCrypt
- [x] **Clientes** - CRUD enterprise com service layer
- [x] **Restaurantes** - CRUD enterprise com service layer

### ✅ Funcionalidades Core
- [x] Clean Architecture implementada
- [x] Service layer em todos os módulos
- [x] Dependency injection configurado
- [x] Entity Framework com migrations
- [x] API REST com Swagger documentation
- [x] Segurança enterprise (JWT + BCrypt)

### 📊 Módulos Disponíveis (Nível Padrão)
- [x] **Categorias** - CRUD básico
- [x] **Produtos** - CRUD básico  
- [x] **Pedidos** - CRUD básico
- [x] **Impressoras** - CRUD básico
- [x] **Relatórios** - Básico

## ��️ Comandos

### Execução
```powershell
# Sistema completo
.\start-pdv.ps1

# Build da solution
dotnet build SistemaPDV.sln
```

### Database
```powershell
# Aplicar migrations
cd Infrastructure\SistemaPDV.Infrastructure
dotnet ef database update --startup-project "../../API/SistemaPDV.API"
```

## 🎯 Padrão de Qualidade

### Enterprise Standard (8.5/10)
- ✅ **Clean Architecture** com separação clara
- ✅ **SOLID Principles** aplicados consistentemente  
- ✅ **Service Layer** robusto em todos os módulos
- ✅ **Security Enterprise** com JWT + BCrypt
- ✅ **Dependency Injection** configurado
- ✅ **Entity Framework** com migrations
- ✅ **API Documentation** com Swagger

## 🚨 Troubleshooting

### Problema: Erro 401 Unauthorized nas páginas
```
✅ NORMAL: Faça LOGIN primeiro!

1. Acesse: http://localhost:5000
2. Clique em "Login" 
3. Use: admin@sistema.com / 123456
4. Agora todas as páginas funcionarão
```

### Problema: Sistema não funciona - LOGIN FALHA
```powershell
# ✅ CORRIGIDO: Use o novo setup automatizado
.\setup-sistema.ps1
# Credenciais: admin@sistema.com / 123456
```

### Problema: API não inicia automaticamente
```powershell
# Solução: Use setup completo ou inicie manualmente
.\setup-sistema.ps1
# OU manualmente:
dotnet run --project "API\SistemaPDV.API" --urls "http://localhost:5001"
```

### Problema: Web não consegue acessar API
```powershell
# 1. Use setup automatizado: .\setup-sistema.ps1
# 2. OU verifique se API está rodando: http://localhost:5001/swagger
# 3. Crie dados iniciais via Swagger: POST /api/seed/criar-dados-iniciais
# 4. Use credenciais: admin@sistema.com / 123456
```

### Problema: Erro JavaScript "blazor-ssr-end"
```
✅ CORRIGIDO: Adicionada configuração anti-erro no blazor-config.js
✅ Configuração automática de error handling
✅ Prevenção de carregamentos duplos
```

### Problema: Formulários sem autocomplete
```
✅ CORRIGIDO: Atributos autocomplete adicionados em todos os formulários
✅ Melhoria na experiência do usuário
✅ Conformidade com diretrizes de acessibilidade
✅ Detalhes: Ver CORRECOES_AUTOCOMPLETE.md
```

### Problema: Build falha
```powershell
# Limpeza completa
dotnet clean SistemaPDV.sln
dotnet build SistemaPDV.sln
```

## 📄 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).

---

**Sistema PDV Enterprise - Arquitetura Clean, Segurança Moderna**

*Desenvolvido com .NET 8 e padrões enterprise*
