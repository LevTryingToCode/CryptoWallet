using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CryptoWallet.Migrations
{
    /// <inheritdoc />
    public partial class EntitiesFirst : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "currencies",
                columns: table => new
                {
                    CurrencyId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Value = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currencies", x => x.CurrencyId);
                });

            migrationBuilder.CreateTable(
                name: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "wallets",
                columns: table => new
                {
                    WalletId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_wallets", x => x.WalletId);
                    table.ForeignKey(
                        name: "FK_wallets_users_UserId",
                        column: x => x.UserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "currencyItems",
                columns: table => new
                {
                    CurrencyItemId = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CurrencyId = table.Column<int>(type: "int", nullable: false),
                    Quantity = table.Column<int>(type: "int", nullable: false),
                    WalletId = table.Column<int>(type: "int", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_currencyItems", x => x.CurrencyItemId);
                    table.ForeignKey(
                        name: "FK_currencyItems_currencies_CurrencyId",
                        column: x => x.CurrencyId,
                        principalTable: "currencies",
                        principalColumn: "CurrencyId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_currencyItems_wallets_WalletId",
                        column: x => x.WalletId,
                        principalTable: "wallets",
                        principalColumn: "WalletId");
                });

            migrationBuilder.CreateIndex(
                name: "IX_currencyItems_CurrencyId",
                table: "currencyItems",
                column: "CurrencyId");

            migrationBuilder.CreateIndex(
                name: "IX_currencyItems_WalletId",
                table: "currencyItems",
                column: "WalletId");

            migrationBuilder.CreateIndex(
                name: "IX_wallets_UserId",
                table: "wallets",
                column: "UserId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "currencyItems");

            migrationBuilder.DropTable(
                name: "currencies");

            migrationBuilder.DropTable(
                name: "wallets");

            migrationBuilder.DropTable(
                name: "users");
        }
    }
}
