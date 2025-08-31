-- Script para criar tabela de Clientes
-- Execute este script no SQL Server Management Studio ou HeidiSQL

USE SistemaPDV;
GO

-- Criar tabela Clientes se não existir
IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = 'Clientes')
BEGIN
    CREATE TABLE Clientes (
        Id int IDENTITY(1,1) PRIMARY KEY,
        Nome nvarchar(100) NOT NULL,
        Email nvarchar(100) NULL,
        Telefone nvarchar(20) NULL,
        CPF nvarchar(11) NULL,
        CNPJ nvarchar(14) NULL,
        DataCadastro datetime2 NOT NULL DEFAULT GETDATE(),
        Ativo bit NOT NULL DEFAULT 1,
        Endereco nvarchar(200) NULL,
        Numero nvarchar(10) NULL,
        Complemento nvarchar(100) NULL,
        Bairro nvarchar(100) NULL,
        Cidade nvarchar(100) NULL,
        UF nvarchar(2) NULL,
        CEP nvarchar(8) NULL,
        Observacoes nvarchar(500) NULL
    );

    -- Criar índices para melhor performance
    CREATE INDEX IX_Clientes_Nome ON Clientes(Nome);
    CREATE INDEX IX_Clientes_Email ON Clientes(Email);
    CREATE INDEX IX_Clientes_CPF ON Clientes(CPF);
    CREATE INDEX IX_Clientes_CNPJ ON Clientes(CNPJ);
    
    PRINT 'Tabela Clientes criada com sucesso!';
END
ELSE
BEGIN
    PRINT 'Tabela Clientes já existe.';
END

-- Inserir alguns dados de exemplo se a tabela estiver vazia
IF NOT EXISTS (SELECT * FROM Clientes)
BEGIN
    INSERT INTO Clientes (Nome, Email, Telefone, CPF, Cidade, UF, Ativo) VALUES
    ('João Silva', 'joao.silva@email.com', '(11) 99999-1111', '11111111111', 'São Paulo', 'SP', 1),
    ('Maria Santos', 'maria.santos@email.com', '(11) 99999-2222', '22222222222', 'São Paulo', 'SP', 1),
    ('Pedro Oliveira', 'pedro.oliveira@email.com', '(11) 99999-3333', '33333333333', 'Rio de Janeiro', 'RJ', 1),
    ('Ana Costa', 'ana.costa@email.com', '(11) 99999-4444', '44444444444', 'Belo Horizonte', 'MG', 1),
    ('Carlos Souza', 'carlos.souza@email.com', '(11) 99999-5555', '55555555555', 'Brasília', 'DF', 1);
    
    PRINT 'Dados de exemplo inseridos com sucesso!';
END

SELECT COUNT(*) as TotalClientes FROM Clientes WHERE Ativo = 1;
GO
