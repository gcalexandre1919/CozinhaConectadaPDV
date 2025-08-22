-- Script SQL para criar todas as tabelas do Sistema PDV
-- Execute este script no HeidiSQL se as tabelas não forem criadas automaticamente

-- Tabela Restaurantes
CREATE TABLE IF NOT EXISTS "Restaurantes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Restaurantes" PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NOT NULL,
    "CNPJ" TEXT NULL,
    "Email" TEXT NULL,
    "Telefone" TEXT NULL,
    "Endereco" TEXT NULL,
    "DataCriacao" TEXT NOT NULL,
    "DataAtualizacao" TEXT NULL,
    "Ativo" INTEGER NOT NULL
);

-- Tabela Categorias
CREATE TABLE IF NOT EXISTS "Categorias" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Categorias" PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NOT NULL,
    "Descricao" TEXT NULL,
    "Cor" TEXT NULL,
    "IconeUrl" TEXT NULL,
    "Ordem" INTEGER NOT NULL,
    "RestauranteId" INTEGER NOT NULL,
    "DataCriacao" TEXT NOT NULL,
    "DataAtualizacao" TEXT NULL,
    "Ativo" INTEGER NOT NULL,
    CONSTRAINT "FK_Categorias_Restaurantes_RestauranteId" FOREIGN KEY ("RestauranteId") REFERENCES "Restaurantes" ("Id") ON DELETE CASCADE
);

-- Tabela Produtos
CREATE TABLE IF NOT EXISTS "Produtos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Produtos" PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NOT NULL,
    "Descricao" TEXT NULL,
    "Preco" TEXT NOT NULL,
    "CategoriaId" INTEGER NULL,
    "RestauranteId" INTEGER NOT NULL,
    "Disponivel" INTEGER NOT NULL,
    "CodigoBarras" TEXT NULL,
    "ImagemUrl" TEXT NULL,
    "Ingredientes" TEXT NULL,
    "InformacoesNutricionais" TEXT NULL,
    "TempoPreparoMinutos" INTEGER NULL,
    "Promocional" INTEGER NOT NULL,
    "PrecoPromocional" TEXT NULL,
    "InicioPromocao" TEXT NULL,
    "FimPromocao" TEXT NULL,
    "DataCriacao" TEXT NOT NULL,
    "DataAtualizacao" TEXT NULL,
    "Ativo" INTEGER NOT NULL,
    CONSTRAINT "FK_Produtos_Categorias_CategoriaId" FOREIGN KEY ("CategoriaId") REFERENCES "Categorias" ("Id"),
    CONSTRAINT "FK_Produtos_Restaurantes_RestauranteId" FOREIGN KEY ("RestauranteId") REFERENCES "Restaurantes" ("Id") ON DELETE CASCADE
);

-- Tabela Clientes
CREATE TABLE IF NOT EXISTS "Clientes" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Clientes" PRIMARY KEY AUTOINCREMENT,
    "Nome" TEXT NOT NULL,
    "Email" TEXT NULL,
    "Telefone" TEXT NULL,
    "CPF" TEXT NULL,
    "DataNascimento" TEXT NULL,
    "RestauranteId" INTEGER NOT NULL,
    "TotalPedidos" INTEGER NOT NULL,
    "TotalGasto" TEXT NOT NULL,
    "UltimoPedido" TEXT NULL,
    "Observacoes" TEXT NULL,
    "DataCriacao" TEXT NOT NULL,
    "DataAtualizacao" TEXT NULL,
    "Ativo" INTEGER NOT NULL,
    CONSTRAINT "FK_Clientes_Restaurantes_RestauranteId" FOREIGN KEY ("RestauranteId") REFERENCES "Restaurantes" ("Id") ON DELETE CASCADE
);

-- Tabela Enderecos
CREATE TABLE IF NOT EXISTS "Enderecos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Enderecos" PRIMARY KEY AUTOINCREMENT,
    "Rua" TEXT NULL,
    "Numero" TEXT NULL,
    "Bairro" TEXT NULL,
    "Cidade" TEXT NULL,
    "Complemento" TEXT NULL,
    "Referencia" TEXT NULL,
    "CEP" TEXT NULL,
    "EnderecoPrincipal" INTEGER NOT NULL,
    "ClienteId" INTEGER NOT NULL,
    "DataCriacao" TEXT NOT NULL,
    "DataAtualizacao" TEXT NULL,
    "Ativo" INTEGER NOT NULL,
    CONSTRAINT "FK_Enderecos_Clientes_ClienteId" FOREIGN KEY ("ClienteId") REFERENCES "Clientes" ("Id") ON DELETE CASCADE
);

-- Tabela Pedidos
CREATE TABLE IF NOT EXISTS "Pedidos" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_Pedidos" PRIMARY KEY AUTOINCREMENT,
    "NumeroPedido" TEXT NOT NULL,
    "Data" TEXT NOT NULL,
    "TipoPedido" INTEGER NOT NULL,
    "ClienteId" INTEGER NULL,
    "EnderecoId" INTEGER NULL,
    "RestauranteId" INTEGER NOT NULL,
    "SubTotal" TEXT NOT NULL,
    "TaxaEntrega" TEXT NOT NULL,
    "TaxaServico" TEXT NOT NULL,
    "PercentualTaxaServico" REAL NOT NULL,
    "ValorTotal" TEXT NOT NULL,
    "ContaFechada" INTEGER NOT NULL,
    "DataFechamento" TEXT NULL,
    "Observacoes" TEXT NULL,
    "DataCriacao" TEXT NOT NULL,
    "DataAtualizacao" TEXT NULL,
    "Ativo" INTEGER NOT NULL,
    CONSTRAINT "FK_Pedidos_Clientes_ClienteId" FOREIGN KEY ("ClienteId") REFERENCES "Clientes" ("Id"),
    CONSTRAINT "FK_Pedidos_Enderecos_EnderecoId" FOREIGN KEY ("EnderecoId") REFERENCES "Enderecos" ("Id"),
    CONSTRAINT "FK_Pedidos_Restaurantes_RestauranteId" FOREIGN KEY ("RestauranteId") REFERENCES "Restaurantes" ("Id") ON DELETE CASCADE
);

-- Tabela ItensPedido
CREATE TABLE IF NOT EXISTS "ItensPedido" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_ItensPedido" PRIMARY KEY AUTOINCREMENT,
    "PedidoId" INTEGER NOT NULL,
    "ProdutoId" INTEGER NOT NULL,
    "Quantidade" INTEGER NOT NULL,
    "PrecoUnitario" TEXT NOT NULL,
    "ValorTotal" TEXT NOT NULL,
    "Observacoes" TEXT NULL,
    "DataCriacao" TEXT NOT NULL,
    "DataAtualizacao" TEXT NULL,
    "Ativo" INTEGER NOT NULL,
    CONSTRAINT "FK_ItensPedido_Pedidos_PedidoId" FOREIGN KEY ("PedidoId") REFERENCES "Pedidos" ("Id") ON DELETE CASCADE,
    CONSTRAINT "FK_ItensPedido_Produtos_ProdutoId" FOREIGN KEY ("ProdutoId") REFERENCES "Produtos" ("Id") ON DELETE CASCADE
);

-- Tabela FilaImpressao
CREATE TABLE IF NOT EXISTS "FilaImpressao" (
    "Id" INTEGER NOT NULL CONSTRAINT "PK_FilaImpressao" PRIMARY KEY AUTOINCREMENT,
    "PedidoId" INTEGER NOT NULL,
    "TipoDocumento" INTEGER NOT NULL,
    "Status" INTEGER NOT NULL DEFAULT 0,
    "Tentativas" INTEGER NOT NULL,
    "DataImpressao" TEXT NULL,
    "MensagemErro" TEXT NULL,
    "Prioritario" INTEGER NOT NULL,
    "DataCriacao" TEXT NOT NULL,
    "DataAtualizacao" TEXT NULL,
    "Ativo" INTEGER NOT NULL,
    CONSTRAINT "FK_FilaImpressao_Pedidos_PedidoId" FOREIGN KEY ("PedidoId") REFERENCES "Pedidos" ("Id") ON DELETE CASCADE
);

-- Criar índices para melhor performance
CREATE INDEX IF NOT EXISTS "IX_Categorias_RestauranteId" ON "Categorias" ("RestauranteId");
CREATE INDEX IF NOT EXISTS "IX_Produtos_CategoriaId" ON "Produtos" ("CategoriaId");
CREATE INDEX IF NOT EXISTS "IX_Produtos_RestauranteId" ON "Produtos" ("RestauranteId");
CREATE INDEX IF NOT EXISTS "IX_Clientes_RestauranteId" ON "Clientes" ("RestauranteId");
CREATE INDEX IF NOT EXISTS "IX_Enderecos_ClienteId" ON "Enderecos" ("ClienteId");
CREATE INDEX IF NOT EXISTS "IX_Pedidos_ClienteId" ON "Pedidos" ("ClienteId");
CREATE INDEX IF NOT EXISTS "IX_Pedidos_EnderecoId" ON "Pedidos" ("EnderecoId");
CREATE INDEX IF NOT EXISTS "IX_Pedidos_RestauranteId" ON "Pedidos" ("RestauranteId");
CREATE INDEX IF NOT EXISTS "IX_ItensPedido_PedidoId" ON "ItensPedido" ("PedidoId");
CREATE INDEX IF NOT EXISTS "IX_ItensPedido_ProdutoId" ON "ItensPedido" ("ProdutoId");
CREATE INDEX IF NOT EXISTS "IX_FilaImpressao_PedidoId" ON "FilaImpressao" ("PedidoId");
