using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GoodFood.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddCustomerFeedbackTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_AspNetUsers_RestaurantOwnerId",
                table: "Offers");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantOwnerId",
                table: "Offers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.CreateTable(
                name: "CustomerFeedbacks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    CustomerName = table.Column<string>(type: "TEXT", nullable: false),
                    Rating = table.Column<int>(type: "INTEGER", nullable: false),
                    Comment = table.Column<string>(type: "TEXT", maxLength: 100, nullable: true),
                    CreatedDate = table.Column<DateTime>(type: "TEXT", nullable: false),
                    RestaurantOwnerId = table.Column<string>(type: "TEXT", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CustomerFeedbacks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CustomerFeedbacks_AspNetUsers_RestaurantOwnerId",
                        column: x => x.RestaurantOwnerId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_CustomerFeedbacks_RestaurantOwnerId",
                table: "CustomerFeedbacks",
                column: "RestaurantOwnerId");

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_AspNetUsers_RestaurantOwnerId",
                table: "Offers",
                column: "RestaurantOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Offers_AspNetUsers_RestaurantOwnerId",
                table: "Offers");

            migrationBuilder.DropTable(
                name: "CustomerFeedbacks");

            migrationBuilder.AlterColumn<string>(
                name: "RestaurantOwnerId",
                table: "Offers",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Offers_AspNetUsers_RestaurantOwnerId",
                table: "Offers",
                column: "RestaurantOwnerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
