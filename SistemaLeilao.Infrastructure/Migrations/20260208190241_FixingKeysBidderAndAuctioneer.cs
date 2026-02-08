using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaLeilao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixingKeysBidderAndAuctioneer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctioneers_Users_UserId",
                table: "Auctioneers");

            migrationBuilder.DropForeignKey(
                name: "FK_Bidders_Users_UserId",
                table: "Bidders");

            migrationBuilder.DropIndex(
                name: "IX_Bidders_UserId",
                table: "Bidders");

            migrationBuilder.DropIndex(
                name: "IX_Auctioneers_UserId",
                table: "Auctioneers");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Auctioneers");

            migrationBuilder.AddColumn<Guid>(
                name: "ExternalId",
                table: "Users",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Auctioneers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .OldAnnotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctioneers_Users_Id",
                table: "Auctioneers",
                column: "Id",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctioneers_Users_Id",
                table: "Auctioneers");

            migrationBuilder.DropColumn(
                name: "ExternalId",
                table: "Users");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Bidders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AlterColumn<long>(
                name: "Id",
                table: "Auctioneers",
                type: "bigint",
                nullable: false,
                oldClrType: typeof(long),
                oldType: "bigint")
                .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn);

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Auctioneers",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_Bidders_UserId",
                table: "Bidders",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctioneers_UserId",
                table: "Auctioneers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctioneers_Users_UserId",
                table: "Auctioneers",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Bidders_Users_UserId",
                table: "Bidders",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
