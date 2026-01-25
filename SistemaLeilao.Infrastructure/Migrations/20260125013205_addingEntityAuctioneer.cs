using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SistemaLeilao.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class addingEntityAuctioneer : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Bidders",
                type: "character varying(15)",
                maxLength: 15,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Bidders",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Bidders",
                type: "character varying(150)",
                maxLength: 150,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<long>(
                name: "UserId",
                table: "Bidders",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<long>(
                name: "AuctioneerId",
                table: "Auctions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "Auctioneers",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Bio = table.Column<string>(type: "character varying(1000)", maxLength: 1000, nullable: true),
                    Rating = table.Column<double>(type: "double precision", nullable: false, defaultValue: 5.0),
                    ExternalId = table.Column<Guid>(type: "uuid", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Version = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "character varying(150)", maxLength: 150, nullable: false),
                    Email = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    PhoneNumber = table.Column<string>(type: "character varying(20)", maxLength: 20, nullable: true),
                    UserId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Auctioneers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Auctioneers_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Bidders_UserId",
                table: "Bidders",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Auctions_AuctioneerId",
                table: "Auctions",
                column: "AuctioneerId");

            migrationBuilder.CreateIndex(
                name: "IX_Auctioneers_UserId",
                table: "Auctioneers",
                column: "UserId",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Auctions_Auctioneers_AuctioneerId",
                table: "Auctions",
                column: "AuctioneerId",
                principalTable: "Auctioneers",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Auctions_Auctioneers_AuctioneerId",
                table: "Auctions");

            migrationBuilder.DropForeignKey(
                name: "FK_Bidders_Users_UserId",
                table: "Bidders");

            migrationBuilder.DropTable(
                name: "Auctioneers");

            migrationBuilder.DropIndex(
                name: "IX_Bidders_UserId",
                table: "Bidders");

            migrationBuilder.DropIndex(
                name: "IX_Auctions_AuctioneerId",
                table: "Auctions");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Bidders");

            migrationBuilder.DropColumn(
                name: "AuctioneerId",
                table: "Auctions");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Bidders",
                type: "character varying(15)",
                maxLength: 15,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(15)",
                oldMaxLength: 15,
                oldNullable: true);
        }
    }
}
