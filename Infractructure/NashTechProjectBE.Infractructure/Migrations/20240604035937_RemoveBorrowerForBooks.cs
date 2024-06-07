using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NashTechProjectBE.Infractructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveBorrowerForBooks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_BorrowedUserId",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "BorrowedUserId",
                table: "Books",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_BorrowedUserId",
                table: "Books",
                newName: "IX_Books_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_UserId",
                table: "Books",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Users_UserId",
                table: "Books");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "Books",
                newName: "BorrowedUserId");

            migrationBuilder.RenameIndex(
                name: "IX_Books_UserId",
                table: "Books",
                newName: "IX_Books_BorrowedUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Users_BorrowedUserId",
                table: "Books",
                column: "BorrowedUserId",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
