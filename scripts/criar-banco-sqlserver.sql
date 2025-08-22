-- Script SQL para criação manual do banco de dados SistemaPDV
-- Execute este script se tiver problemas com as migrações automáticas

-- Criar o banco de dados (execute em master ou usando HeidiSQL)
IF NOT EXISTS (SELECT name FROM sys.databases WHERE name = N'SistemaPDV')
BEGIN
    CREATE DATABASE [SistemaPDV]
END
GO

USE [SistemaPDV]
GO

-- Verificar se o banco já tem tabelas (evitar recriar)
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'AspNetUsers')
BEGIN
    -- Tabelas do Identity
    CREATE TABLE [AspNetRoles] (
        [Id] nvarchar(450) NOT NULL,
        [Name] nvarchar(256) NULL,
        [NormalizedName] nvarchar(256) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoles] PRIMARY KEY ([Id])
    );

    CREATE TABLE [AspNetUsers] (
        [Id] nvarchar(450) NOT NULL,
        [UserName] nvarchar(256) NULL,
        [NormalizedUserName] nvarchar(256) NULL,
        [Email] nvarchar(256) NULL,
        [NormalizedEmail] nvarchar(256) NULL,
        [EmailConfirmed] bit NOT NULL,
        [PasswordHash] nvarchar(max) NULL,
        [SecurityStamp] nvarchar(max) NULL,
        [ConcurrencyStamp] nvarchar(max) NULL,
        [PhoneNumber] nvarchar(max) NULL,
        [PhoneNumberConfirmed] bit NOT NULL,
        [TwoFactorEnabled] bit NOT NULL,
        [LockoutEnd] datetimeoffset NULL,
        [LockoutEnabled] bit NOT NULL,
        [AccessFailedCount] int NOT NULL,
        [RestauranteId] int NULL,
        CONSTRAINT [PK_AspNetUsers] PRIMARY KEY ([Id])
    );

    CREATE TABLE [AspNetRoleClaims] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetRoleClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetRoleClaims_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE
    );

    CREATE TABLE [AspNetUserClaims] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [UserId] nvarchar(450) NOT NULL,
        [ClaimType] nvarchar(max) NULL,
        [ClaimValue] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserClaims] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_AspNetUserClaims_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE TABLE [AspNetUserLogins] (
        [LoginProvider] nvarchar(450) NOT NULL,
        [ProviderKey] nvarchar(450) NOT NULL,
        [ProviderDisplayName] nvarchar(max) NULL,
        [UserId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserLogins] PRIMARY KEY ([LoginProvider], [ProviderKey]),
        CONSTRAINT [FK_AspNetUserLogins_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE TABLE [AspNetUserRoles] (
        [UserId] nvarchar(450) NOT NULL,
        [RoleId] nvarchar(450) NOT NULL,
        CONSTRAINT [PK_AspNetUserRoles] PRIMARY KEY ([UserId], [RoleId]),
        CONSTRAINT [FK_AspNetUserRoles_AspNetRoles_RoleId] FOREIGN KEY ([RoleId]) REFERENCES [AspNetRoles] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_AspNetUserRoles_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    CREATE TABLE [AspNetUserTokens] (
        [UserId] nvarchar(450) NOT NULL,
        [LoginProvider] nvarchar(450) NOT NULL,
        [Name] nvarchar(450) NOT NULL,
        [Value] nvarchar(max) NULL,
        CONSTRAINT [PK_AspNetUserTokens] PRIMARY KEY ([UserId], [LoginProvider], [Name]),
        CONSTRAINT [FK_AspNetUserTokens_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE
    );

    -- Tabela Restaurantes
    CREATE TABLE [Restaurantes] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Nome] nvarchar(200) NOT NULL,
        [Email] nvarchar(200) NOT NULL,
        [Senha] nvarchar(500) NOT NULL,
        [LogoUrl] nvarchar(500) NULL,
        [Telefone] nvarchar(20) NULL,
        [Endereco] nvarchar(500) NULL,
        [CNPJ] nvarchar(18) NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataAtualizacao] datetime2 NULL,
        CONSTRAINT [PK_Restaurantes] PRIMARY KEY ([Id])
    );

    -- Tabela Produtos
    CREATE TABLE [Produtos] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Nome] nvarchar(200) NOT NULL,
        [Descricao] nvarchar(1000) NULL,
        [Preco] decimal(10,2) NOT NULL,
        [Categoria] nvarchar(100) NULL,
        [ImagemUrl] nvarchar(500) NULL,
        [Ativo] bit NOT NULL,
        [RestauranteId] int NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataAtualizacao] datetime2 NULL,
        CONSTRAINT [PK_Produtos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Produtos_Restaurantes_RestauranteId] FOREIGN KEY ([RestauranteId]) REFERENCES [Restaurantes] ([Id]) ON DELETE CASCADE
    );

    -- Tabela Clientes
    CREATE TABLE [Clientes] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Nome] nvarchar(200) NOT NULL,
        [Telefone] nvarchar(20) NOT NULL,
        [CPF] nvarchar(14) NULL,
        [RestauranteId] int NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataAtualizacao] datetime2 NULL,
        CONSTRAINT [PK_Clientes] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Clientes_Restaurantes_RestauranteId] FOREIGN KEY ([RestauranteId]) REFERENCES [Restaurantes] ([Id]) ON DELETE CASCADE
    );

    -- Tabela Enderecos
    CREATE TABLE [Enderecos] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Rua] nvarchar(200) NULL,
        [Numero] nvarchar(20) NULL,
        [Bairro] nvarchar(100) NULL,
        [Cidade] nvarchar(100) NULL,
        [Complemento] nvarchar(200) NULL,
        [Referencia] nvarchar(200) NULL,
        [CEP] nvarchar(10) NULL,
        [ClienteId] int NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataAtualizacao] datetime2 NULL,
        CONSTRAINT [PK_Enderecos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Enderecos_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE CASCADE
    );

    -- Tabela Pedidos
    CREATE TABLE [Pedidos] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [NumeroPedido] nvarchar(50) NOT NULL,
        [TipoPedido] int NOT NULL,
        [SubTotal] decimal(10,2) NOT NULL,
        [TaxaServico] decimal(10,2) NOT NULL,
        [TaxaEntrega] decimal(10,2) NOT NULL,
        [ValorTotal] decimal(10,2) NOT NULL,
        [Status] int NOT NULL,
        [Observacoes] nvarchar(1000) NULL,
        [ClienteId] int NOT NULL,
        [RestauranteId] int NOT NULL,
        [EnderecoId] int NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataAtualizacao] datetime2 NULL,
        CONSTRAINT [PK_Pedidos] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_Pedidos_Clientes_ClienteId] FOREIGN KEY ([ClienteId]) REFERENCES [Clientes] ([Id]) ON DELETE NO ACTION,
        CONSTRAINT [FK_Pedidos_Restaurantes_RestauranteId] FOREIGN KEY ([RestauranteId]) REFERENCES [Restaurantes] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_Pedidos_Enderecos_EnderecoId] FOREIGN KEY ([EnderecoId]) REFERENCES [Enderecos] ([Id]) ON DELETE SET NULL
    );

    -- Tabela ItensPedido
    CREATE TABLE [ItensPedido] (
        [Id] int IDENTITY(1,1) NOT NULL,
        [Quantidade] int NOT NULL,
        [PrecoUnitario] decimal(10,2) NOT NULL,
        [PrecoTotal] decimal(10,2) NOT NULL,
        [Observacoes] nvarchar(500) NULL,
        [PedidoId] int NOT NULL,
        [ProdutoId] int NOT NULL,
        [DataCriacao] datetime2 NOT NULL,
        [DataAtualizacao] datetime2 NULL,
        CONSTRAINT [PK_ItensPedido] PRIMARY KEY ([Id]),
        CONSTRAINT [FK_ItensPedido_Pedidos_PedidoId] FOREIGN KEY ([PedidoId]) REFERENCES [Pedidos] ([Id]) ON DELETE CASCADE,
        CONSTRAINT [FK_ItensPedido_Produtos_ProdutoId] FOREIGN KEY ([ProdutoId]) REFERENCES [Produtos] ([Id]) ON DELETE NO ACTION
    );

    -- Índices
    CREATE INDEX [IX_AspNetRoleClaims_RoleId] ON [AspNetRoleClaims] ([RoleId]);
    CREATE UNIQUE INDEX [RoleNameIndex] ON [AspNetRoles] ([NormalizedName]) WHERE [NormalizedName] IS NOT NULL;
    CREATE INDEX [IX_AspNetUserClaims_UserId] ON [AspNetUserClaims] ([UserId]);
    CREATE INDEX [IX_AspNetUserLogins_UserId] ON [AspNetUserLogins] ([UserId]);
    CREATE INDEX [IX_AspNetUserRoles_RoleId] ON [AspNetUserRoles] ([RoleId]);
    CREATE INDEX [EmailIndex] ON [AspNetUsers] ([NormalizedEmail]);
    CREATE UNIQUE INDEX [UserNameIndex] ON [AspNetUsers] ([NormalizedUserName]) WHERE [NormalizedUserName] IS NOT NULL;
    CREATE INDEX [IX_AspNetUsers_RestauranteId] ON [AspNetUsers] ([RestauranteId]);
    CREATE INDEX [IX_Clientes_RestauranteId] ON [Clientes] ([RestauranteId]);
    CREATE INDEX [IX_Enderecos_ClienteId] ON [Enderecos] ([ClienteId]);
    CREATE INDEX [IX_ItensPedido_PedidoId] ON [ItensPedido] ([PedidoId]);
    CREATE INDEX [IX_ItensPedido_ProdutoId] ON [ItensPedido] ([ProdutoId]);
    CREATE INDEX [IX_Pedidos_ClienteId] ON [Pedidos] ([ClienteId]);
    CREATE INDEX [IX_Pedidos_EnderecoId] ON [Pedidos] ([EnderecoId]);
    CREATE UNIQUE INDEX [IX_Pedidos_NumeroPedido] ON [Pedidos] ([NumeroPedido]);
    CREATE INDEX [IX_Pedidos_RestauranteId] ON [Pedidos] ([RestauranteId]);
    CREATE INDEX [IX_Produtos_RestauranteId] ON [Produtos] ([RestauranteId]);
    CREATE UNIQUE INDEX [IX_Restaurantes_Email] ON [Restaurantes] ([Email]);

    -- Adicionar FK para AspNetUsers -> Restaurantes
    ALTER TABLE [AspNetUsers] ADD CONSTRAINT [FK_AspNetUsers_Restaurantes_RestauranteId] 
        FOREIGN KEY ([RestauranteId]) REFERENCES [Restaurantes] ([Id]) ON DELETE SET NULL;

    PRINT 'Banco de dados SistemaPDV criado com sucesso!'
END
ELSE
BEGIN
    PRINT 'Banco de dados SistemaPDV já existe.'
END
GO
