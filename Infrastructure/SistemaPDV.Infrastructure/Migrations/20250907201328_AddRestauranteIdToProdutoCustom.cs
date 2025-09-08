using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPDV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestauranteIdToProdutoCustom : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RestauranteId",
                table: "Produtos",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Produtos_RestauranteId",
                table: "Produtos",
                column: "RestauranteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Produtos_Restaurantes_RestauranteId",
                table: "Produtos",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Produtos_Restaurantes_RestauranteId",
                table: "Produtos");

            migrationBuilder.DropIndex(
                name: "IX_Produtos_RestauranteId",
                table: "Produtos");

            migrationBuilder.DropColumn(
                name: "RestauranteId",
                table: "Produtos");
        }
    }
}
