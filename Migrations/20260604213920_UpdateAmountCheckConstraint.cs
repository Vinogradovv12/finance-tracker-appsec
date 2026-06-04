using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FinanceTracker.api.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAmountCheckConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Transaction_Amount",
                table: "Transactions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transaction_Amount",
                table: "Transactions",
                sql: "\"Amount\" > 0");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropCheckConstraint(
                name: "CK_Transaction_Amount",
                table: "Transactions");

            migrationBuilder.AddCheckConstraint(
                name: "CK_Transaction_Amount",
                table: "Transactions",
                sql: "\"Amount\" >= 0");
        }
    }
}
