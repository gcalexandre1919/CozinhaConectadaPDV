# 🍽️ Sistema PDV - Restaurante

Sistema de Ponto de Venda (PDV) completo para restaurantes, desenvolvido em C# com ASP.NET Core e **SQL Server**.

## � Início Rápido

### 1. Pré-requisitos
- .NET 8.0 SDK
- SQL Server Express (ou LocalDB)
- HeidiSQL (opcional, para visualizar banco)

### 2. Executar o Sistema

**Opção Automática (Recomendada):**
```powershell
.\executar-sistema.ps1
```

**Opção Manual:**
```powershell
# Terminal 1 - API
cd src\API\SistemaPDV.API
dotnet run

# Terminal 2 - Frontend  
cd src\Web\SistemaPDV.Web
dotnet run
```

### 3. Acessar o Sistema
- **Frontend:** http://localhost:5107
- **API/Swagger:** http://localhost:5000/swagger

## 💾 Banco de Dados SQL Server

### Configuração Automática
O sistema criará automaticamente o banco `SistemaPDV` no SQL Server.

### Conectar com HeidiSQL
- **Tipo:** Microsoft SQL Server (TCP/IP)
- **Servidor:** `.\SQLEXPRESS` (ou `localhost` para SQL Server completo)
- **Autenticação:** Windows Authentication
- **Banco:** SistemaPDV

### Configuração Manual (se necessário)
Execute o script: `scripts\criar-banco-sqlserver.sql`

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

## 🔧 Estrutura do Projeto

```
src/
├── Core/               # Entidades e interfaces (Clean Architecture)
├── Infrastructure/     # Banco de dados e repositórios  
├── API/               # Web API (Backend)
└── Web/               # Blazor Server (Frontend)
```

## �️ Tecnologias

- **Backend:** ASP.NET Core 8.0, Entity Framework Core, SQL Server
- **Frontend:** Blazor Server, Bootstrap 5
- **Autenticação:** JWT + ASP.NET Identity
- **Padrões:** Clean Architecture, Repository Pattern, SOLID

## 🎯 Recursos

- ✅ Multi-tenant (cada restaurante isolado)
- ✅ Três tipos de pedido (Balcão, Delivery, Mesa)
- ✅ Cálculo automático de taxas (entrega e serviço)
- ✅ Relatórios completos de vendas
- ✅ Interface responsiva e moderna
- ✅ API documentada (Swagger)
- ✅ Autenticação segura

## 📞 Suporte

Para problemas:
1. Verifique se o SQL Server está rodando
2. Confirme as strings de conexão
3. Consulte `INSTRUCOES_EXECUCAO.md` para detalhes completos

---
Desenvolvido com ❤️ usando C# e ASP.NET Core + SQL Server

