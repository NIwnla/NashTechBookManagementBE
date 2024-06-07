using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NashTechProjectBE.Infractructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "CountResetDate",
                table: "Users",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "RequestCount",
                table: "Users",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CountResetDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RequestCount",
                table: "Users");
        }
    }
}
