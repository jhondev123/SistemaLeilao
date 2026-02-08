using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SistemaLeilao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixShadowProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Auctioneers_AuctioneerId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Bidders_BidderId",
                table: "Auctions");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_BidderId",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "BidderId",
                table: "Auctions");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Auctioneers_AuctioneerId",
                table: "Auctions",
                column: "AuctioneerId",
                principalTable: "Auctioneers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Auctioneers_AuctioneerId",
                table: "Auctions");

            migrationBuilder.AddColumn<long>(
                name: "BidderId",
                table: "Auctions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_BidderId",
                table: "Auctions",
                column: "BidderId");

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Auctioneers_AuctioneerId",
                table: "Auctions",
                column: "AuctioneerId",
                principalTable: "Auctioneers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Bidders_BidderId",
                table: "Auctions",
                column: "BidderId",
                principalTable: "Bidders",
                principalColumn: "Id");
        }
    }
}
