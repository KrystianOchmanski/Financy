using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferGroupAndDirection : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TransferDirection",
                table: "Transactions",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "TransferGroupId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TransferDirection",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "TransferGroupId",
                table: "Transactions");
        }
    }
}
