using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class vsdf : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_reviews_AspNetUsers_UserID",
                table: "reviews");

            migrationBuilder.DropIndex(
                name: "IX_reviews_UserID",
                table: "reviews");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "reviews");

            migrationBuilder.AddColumn<int>(
                name: "reviewid",
                table: "AspNetUsers",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_reviewid",
                table: "AspNetUsers",
                column: "reviewid");

            migrationBuilder.AddForeignKey(
                name: "FK_AspNetUsers_reviews_reviewid",
                table: "AspNetUsers",
                column: "reviewid",
                principalTable: "reviews",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_AspNetUsers_reviews_reviewid",
                table: "AspNetUsers");

            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_reviewid",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "reviewid",
                table: "AspNetUsers");

            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "reviews",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateIndex(
                name: "IX_reviews_UserID",
                table: "reviews",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_reviews_AspNetUsers_UserID",
                table: "reviews",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
