using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPDV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddIdentityTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Nome",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "Valor",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "DataNascimento",
                table: "Clientes");

            migrationBuilder.RenameColumn(
                name: "Ativo",
                table: "Impressoras",
                newName: "Tipo");

            migrationBuilder.RenameColumn(
                name: "Processado",
                table: "FilaImpressao",
                newName: "TentativasImpressao");

            migrationBuilder.AddColumn<string>(
                name: "Bairro",
                table: "Restaurantes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "CEP",
                table: "Restaurantes",
                type: "TEXT",
                maxLength: 8,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Cidade",
                table: "Restaurantes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Complemento",
                table: "Restaurantes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCadastro",
                table: "Restaurantes",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Restaurantes",
                type: "TEXT",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Numero",
                table: "Restaurantes",
                type: "TEXT",
                maxLength: 10,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UF",
                table: "Restaurantes",
                type: "TEXT",
                maxLength: 2,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "AbrirGaveta",
                table: "Impressoras",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "Ativa",
                table: "Impressoras",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "CortarPapel",
                table: "Impressoras",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataCadastro",
                table: "Impressoras",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "LarguraPapel",
                table: "Impressoras",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "Observacoes",
                table: "Impressoras",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DataProcessamento",
                table: "FilaImpressao",
                type: "TEXT",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImpressoraId",
                table: "FilaImpressao",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MensagemErro",
                table: "FilaImpressao",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "PedidoId",
                table: "FilaImpressao",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "FilaImpressao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "AbrirGavetaAutomaticamente",
                table: "ConfiguracoesImpressao",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "CabecalhoPersonalizado",
                table: "ConfiguracoesImpressao",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "CortarPapelAutomaticamente",
                table: "ConfiguracoesImpressao",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ImpressoraPadraoId",
                table: "ConfiguracoesImpressao",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ImprimirAutomaticamente",
                table: "ConfiguracoesImpressao",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MargemEsquerda",
                table: "ConfiguracoesImpressao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "MargemSuperior",
                table: "ConfiguracoesImpressao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "NumeroVias",
                table: "ConfiguracoesImpressao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RodapePersonalizado",
                table: "ConfiguracoesImpressao",
                type: "TEXT",
                maxLength: 500,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "TamanhoFonte",
                table: "ConfiguracoesImpressao",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_FilaImpressao_ImpressoraId",
                table: "FilaImpressao",
                column: "ImpressoraId");

            migrationBuilder.CreateIndex(
                name: "IX_FilaImpressao_PedidoId",
                table: "FilaImpressao",
                column: "PedidoId");

            migrationBuilder.CreateIndex(
                name: "IX_ConfiguracoesImpressao_ImpressoraPadraoId",
                table: "ConfiguracoesImpressao",
                column: "ImpressoraPadraoId");

            migrationBuilder.AddForeignKey(
                name: "FK_ConfiguracoesImpressao_Impressoras_ImpressoraPadraoId",
                table: "ConfiguracoesImpressao",
                column: "ImpressoraPadraoId",
                principalTable: "Impressoras",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FilaImpressao_Impressoras_ImpressoraId",
                table: "FilaImpressao",
                column: "ImpressoraId",
                principalTable: "Impressoras",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_FilaImpressao_Pedidos_PedidoId",
                table: "FilaImpressao",
                column: "PedidoId",
                principalTable: "Pedidos",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ConfiguracoesImpressao_Impressoras_ImpressoraPadraoId",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropForeignKey(
                name: "FK_FilaImpressao_Impressoras_ImpressoraId",
                table: "FilaImpressao");

            migrationBuilder.DropForeignKey(
                name: "FK_FilaImpressao_Pedidos_PedidoId",
                table: "FilaImpressao");

            migrationBuilder.DropIndex(
                name: "IX_FilaImpressao_ImpressoraId",
                table: "FilaImpressao");

            migrationBuilder.DropIndex(
                name: "IX_FilaImpressao_PedidoId",
                table: "FilaImpressao");

            migrationBuilder.DropIndex(
                name: "IX_ConfiguracoesImpressao_ImpressoraPadraoId",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "Bairro",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "CEP",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "Cidade",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "Complemento",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "Numero",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "UF",
                table: "Restaurantes");

            migrationBuilder.DropColumn(
                name: "AbrirGaveta",
                table: "Impressoras");

            migrationBuilder.DropColumn(
                name: "Ativa",
                table: "Impressoras");

            migrationBuilder.DropColumn(
                name: "CortarPapel",
                table: "Impressoras");

            migrationBuilder.DropColumn(
                name: "DataCadastro",
                table: "Impressoras");

            migrationBuilder.DropColumn(
                name: "LarguraPapel",
                table: "Impressoras");

            migrationBuilder.DropColumn(
                name: "Observacoes",
                table: "Impressoras");

            migrationBuilder.DropColumn(
                name: "DataProcessamento",
                table: "FilaImpressao");

            migrationBuilder.DropColumn(
                name: "ImpressoraId",
                table: "FilaImpressao");

            migrationBuilder.DropColumn(
                name: "MensagemErro",
                table: "FilaImpressao");

            migrationBuilder.DropColumn(
                name: "PedidoId",
                table: "FilaImpressao");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "FilaImpressao");

            migrationBuilder.DropColumn(
                name: "AbrirGavetaAutomaticamente",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "CabecalhoPersonalizado",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "CortarPapelAutomaticamente",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "ImpressoraPadraoId",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "ImprimirAutomaticamente",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "MargemEsquerda",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "MargemSuperior",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "NumeroVias",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "RodapePersonalizado",
                table: "ConfiguracoesImpressao");

            migrationBuilder.DropColumn(
                name: "TamanhoFonte",
                table: "ConfiguracoesImpressao");

            migrationBuilder.RenameColumn(
                name: "Tipo",
                table: "Impressoras",
                newName: "Ativo");

            migrationBuilder.RenameColumn(
                name: "TentativasImpressao",
                table: "FilaImpressao",
                newName: "Processado");

            migrationBuilder.AddColumn<string>(
                name: "Nome",
                table: "ConfiguracoesImpressao",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Valor",
                table: "ConfiguracoesImpressao",
                type: "TEXT",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "DataNascimento",
                table: "Clientes",
                type: "TEXT",
                nullable: true);
        }
    }
}
