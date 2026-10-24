using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Financy.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddUniqueTransferGroupDirectionIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransferGroupId_TransferDirection",
                table: "Transactions",
                columns: new[] { "TransferGroupId", "TransferDirection" },
                unique: true,
                filter: "[TransferGroupId] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Transactions_TransferGroupId_TransferDirection",
                table: "Transactions");
        }
    }
}
