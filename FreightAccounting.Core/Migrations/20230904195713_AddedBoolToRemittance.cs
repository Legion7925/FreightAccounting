using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreightAccounting.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddedBoolToRemittance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsUserCutEnteredByHand",
                table: "Remittances",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUserCutEnteredByHand",
                table: "Remittances");
        }
    }
}
