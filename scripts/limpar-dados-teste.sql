-- Script para limpar dados de teste do banco SistemaPDV
-- Execute este script quando quiser começar com dados limpos

USE [SistemaPDV]
GO

-- Desabilitar verificação de FK temporariamente
EXEC sp_msforeachtable "ALTER TABLE ? NOCHECK CONSTRAINT all"
GO

-- Limpar dados das tabelas (manter estrutura)
DELETE FROM [ItensPedido]
DELETE FROM [Pedidos]
DELETE FROM [Enderecos]
DELETE FROM [Clientes]
DELETE FROM [Produtos]
DELETE FROM [AspNetUserTokens]
DELETE FROM [AspNetUserRoles]
DELETE FROM [AspNetUserLogins]
DELETE FROM [AspNetUserClaims]
DELETE FROM [AspNetRoleClaims]
DELETE FROM [AspNetUsers]
DELETE FROM [AspNetRoles]
DELETE FROM [Restaurantes]

-- Reabilitar verificação de FK
EXEC sp_msforeachtable "ALTER TABLE ? WITH CHECK CHECK CONSTRAINT all"
GO

-- Resetar IDENTITY das tabelas
DBCC CHECKIDENT('Restaurantes', RESEED, 0)
DBCC CHECKIDENT('Produtos', RESEED, 0)
DBCC CHECKIDENT('Clientes', RESEED, 0)
DBCC CHECKIDENT('Enderecos', RESEED, 0)
DBCC CHECKIDENT('Pedidos', RESEED, 0)
DBCC CHECKIDENT('ItensPedido', RESEED, 0)
DBCC CHECKIDENT('AspNetRoleClaims', RESEED, 0)
DBCC CHECKIDENT('AspNetUserClaims', RESEED, 0)

PRINT 'Dados de teste removidos com sucesso! Banco limpo e pronto para uso.'
GO
