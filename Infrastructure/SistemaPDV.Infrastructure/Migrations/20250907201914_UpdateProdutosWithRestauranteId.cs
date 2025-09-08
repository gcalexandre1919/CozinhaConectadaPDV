using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaPDV.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateProdutosWithRestauranteId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Atualizar produtos existentes com RestauranteId = 1 (restaurante padrão)
            migrationBuilder.Sql("UPDATE Produtos SET RestauranteId = 1 WHERE RestauranteId IS NULL;");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
