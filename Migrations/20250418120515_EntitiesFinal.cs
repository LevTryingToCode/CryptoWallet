using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWallet.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesFinal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Quantity",
                table: "currencyItems");

            migrationBuilder.AddColumn<double>(
                name: "BuyValue",
                table: "currencyItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BuyValue",
                table: "currencyItems");

            migrationBuilder.AddColumn<int>(
                name: "Quantity",
                table: "currencyItems",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
