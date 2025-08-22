# 🍽️ Cozinha Conectada - Sistema PDV

Sistema de Ponto de Venda (PDV) completo para restaurantes e estabelecimentos alimentícios, desenvolvido em C# com ASP.NET Core e **SQL Server**.

## 🚀 Início Rápido

### 1. Pré-requisitos
- .NET 8.0 SDK
- SQL Server Express (ou LocalDB)
- HeidiSQL (opcional, para visualizar banco)

### 2. Executar o Sistema

**Opção Automática (Recomendada):**
```powershell
.\executar-projeto.ps1
```

**Opções Específicas:**
```powershell
.\executar-projeto.ps1 -Acao api        # Apenas API
.\executar-projeto.ps1 -Acao web        # Apenas Web
.\executar-projeto.ps1 -Acao sqlserver  # Com SQL Server
```

### 3. Acessar o Sistema
- **Frontend:** http://localhost:5107
- **API/Swagger:** http://localhost:5000/swagger

## 📁 Estrutura do Projeto

```
📂 CozinhaConectada/
├── 📂 API/                    # API REST
│   └── SistemaPDV.API/
├── 📂 Core/                   # Lógica de negócio
│   └── SistemaPDV.Core/
├── 📂 Infrastructure/         # Acesso a dados
│   └── SistemaPDV.Infrastructure/
├── 📂 Web/                    # Interface web
│   └── SistemaPDV.Web/
├── 📂 scripts/                # Scripts SQL
├── 📂 tools/                  # Ferramentas e scripts
├── 📂 docs/                   # Documentação
├── 🔧 executar-projeto.ps1    # Script principal
└── 📄 SistemaPDV.sln         # Solution
```

## 💾 Banco de Dados SQL Server

### Configuração Automática
O sistema criará automaticamente o banco `SistemaPDV` no SQL Server.

### Conectar com HeidiSQL
- **Tipo:** Microsoft SQL Server (TCP/IP)
- **Servidor:** `.\SQLEXPRESS` (ou `localhost` para SQL Server completo)
- **Autenticação:** Windows Authentication
- **Banco:** SistemaPDV

## 📱 Como Usar

### 1. Primeiro Acesso
1. Acesse http://localhost:5107/register
2. Cadastre seu restaurante
3. Faça login

### 2. Funcionalidades Principais
- **Dashboard:** Visão geral de vendas e métricas
- **Produtos:** Cadastro do cardápio (pratos, bebidas, etc.)
- **Clientes:** Gestão de clientes e endereços
- **Pedidos:** Criação de pedidos (Balcão, Delivery, Mesa)
- **Relatórios:** Análises detalhadas de vendas

## 🛠️ Funcionalidades

- ✅ Gestão de produtos e categorias
- ✅ Sistema de pedidos
- ✅ Impressão de comandas
- ✅ Relatórios de vendas
- ✅ Interface web responsiva
- ✅ API REST completa
- ✅ Multi-tenant (cada restaurante isolado)
- ✅ Três tipos de pedido (Balcão, Delivery, Mesa)
- ✅ Autenticação segura (JWT + ASP.NET Identity)

## 🔧 Tecnologias

- **Backend:** ASP.NET Core 8.0, Entity Framework Core, SQL Server
- **Frontend:** Blazor Server, Bootstrap 5
- **Autenticação:** JWT + ASP.NET Identity
- **Padrões:** Clean Architecture, Repository Pattern, SOLID

## 📚 Documentação

- [Instruções de Execução](docs/INSTRUCOES_EXECUCAO.md)
- [Guia HeidiSQL](docs/GUIA_HEIDISQL.md)
- [README PDV](docs/README-PDV.md)

## 🔧 Ferramentas

- [Gerenciador de Dados](tools/gerenciador-dados.html)
- Scripts PowerShell em `tools/`
- Scripts SQL em `scripts/`

## 📞 Suporte

Para problemas:
1. Verifique se o SQL Server está rodando
2. Confirme as strings de conexão
3. Consulte a documentação em `docs/` para detalhes completos

---
Desenvolvido com ❤️ usando C# e ASP.NET Core + SQL Server

