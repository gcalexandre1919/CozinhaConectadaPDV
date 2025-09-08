using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPDV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRestauranteIdToCategoria : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RestauranteId",
                table: "Categorias",
                type: "INTEGER",
                nullable: false,
                defaultValue: 1);

            migrationBuilder.CreateIndex(
                name: "IX_Categorias_RestauranteId",
                table: "Categorias",
                column: "RestauranteId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categorias_Restaurantes_RestauranteId",
                table: "Categorias",
                column: "RestauranteId",
                principalTable: "Restaurantes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categorias_Restaurantes_RestauranteId",
                table: "Categorias");

            migrationBuilder.DropIndex(
                name: "IX_Categorias_RestauranteId",
                table: "Categorias");

            migrationBuilder.DropColumn(
                name: "RestauranteId",
                table: "Categorias");
        }
    }
}
