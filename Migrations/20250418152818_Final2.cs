using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWallet.Migrations
{
    /// <inheritdoc />
    public partial class Final2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_currencyItems_wallets_WalletId",
                table: "currencyItems");

            migrationBuilder.AlterColumn<int>(
                name: "WalletId",
                table: "currencyItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_currencyItems_wallets_WalletId",
                table: "currencyItems",
                column: "WalletId",
                principalTable: "wallets",
                principalColumn: "WalletId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_currencyItems_wallets_WalletId",
                table: "currencyItems");

            migrationBuilder.AlterColumn<int>(
                name: "WalletId",
                table: "currencyItems",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_currencyItems_wallets_WalletId",
                table: "currencyItems",
                column: "WalletId",
                principalTable: "wallets",
                principalColumn: "WalletId");
        }
    }
}
