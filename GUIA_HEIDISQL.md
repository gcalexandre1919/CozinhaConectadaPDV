# 📋 GUIA: Como Abrir o Banco SQLite no HeidiSQL

## 🔧 Configuração da Conexão

### 1. Abrir HeidiSQL
- Inicie o HeidiSQL
- Clique em "New" (Nova conexão)

### 2. Configurar Conexão SQLite
- **Network type:** SQLite
- **Filename:** Clique em "..." e navegue até:
  `C:\Users\GC\Documents\GitHub\CozinhaConectada\SistemaPDV\src\API\SistemaPDV.API\SistemaPDV.db`
- **Session name:** Sistema PDV
- Clique em "Save" e depois "Open"

### 3. Explorar o Banco
Após conectar, você verá as tabelas:
- **Restaurantes** - Dados dos restaurantes
- **Produtos** - Catálogo de produtos
- **Clientes** - Cadastro de clientes  
- **Enderecos** - Endereços dos clientes
- **Pedidos** - Histórico de pedidos
- **ItensPedido** - Itens dos pedidos
- **Categorias** - Categorias de produtos
- **__EFMigrationsHistory** - Controle de migrações

### 4. Consultas Úteis
```sql
-- Ver todos os produtos
SELECT * FROM Produtos WHERE Ativo = 1;

-- Ver todos os restaurantes
SELECT * FROM Restaurantes WHERE Ativo = 1;

-- Ver produtos com suas categorias
SELECT p.Nome as Produto, c.Nome as Categoria, p.Preco 
FROM Produtos p 
LEFT JOIN Categorias c ON p.CategoriaId = c.Id 
WHERE p.Ativo = 1;

-- Contar registros por tabela
SELECT 'Produtos' as Tabela, COUNT(*) as Total FROM Produtos WHERE Ativo = 1
UNION ALL
SELECT 'Clientes', COUNT(*) FROM Clientes WHERE Ativo = 1
UNION ALL  
SELECT 'Restaurantes', COUNT(*) FROM Restaurantes WHERE Ativo = 1;
```

## 🔍 Alternativas ao HeidiSQL

### DB Browser for SQLite (Gratuito)
- Download: https://sqlitebrowser.org/
- Mais simples e específico para SQLite
- Interface mais amigável para iniciantes

### DBeaver (Gratuito)
- Download: https://dbeaver.io/
- Suporta vários tipos de banco
- Interface mais robusta

## 📍 Localização do Banco
**Caminho completo:**
`C:\Users\GC\Documents\GitHub\CozinhaConectada\SistemaPDV\src\API\SistemaPDV.API\SistemaPDV.db`
