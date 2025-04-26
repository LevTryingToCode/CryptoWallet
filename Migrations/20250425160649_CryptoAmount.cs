using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWallet.Migrations
{
    /// <inheritdoc />
    public partial class CryptoAmount : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "CryptoAmount",
                table: "currencyItems",
                type: "float",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CryptoAmount",
                table: "currencyItems");
        }
    }
}
