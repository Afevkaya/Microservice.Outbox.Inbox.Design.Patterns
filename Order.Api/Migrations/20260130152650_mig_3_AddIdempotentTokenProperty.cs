using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Order.Api.Migrations
{
    /// <inheritdoc />
    public partial class mig_3_AddIdempotentTokenProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Id",
                table: "OrderOutboxes",
                newName: "IdempotentToken");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IdempotentToken",
                table: "OrderOutboxes",
                newName: "Id");
        }
    }
}
