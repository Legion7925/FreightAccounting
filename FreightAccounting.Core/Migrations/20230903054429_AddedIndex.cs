using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreightAccounting.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddedIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "RemittanceNumber",
                table: "Remittances",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductInsuranceNumber",
                table: "Remittances",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DriverLastName",
                table: "Debtors",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "DriverFirstName",
                table: "Debtors",
                type: "nvarchar(450)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Remittances_RemittanceNumber",
                table: "Remittances",
                column: "RemittanceNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Debtors_DriverFirstName",
                table: "Debtors",
                column: "DriverFirstName");

            migrationBuilder.CreateIndex(
                name: "IX_Debtors_DriverLastName",
                table: "Debtors",
                column: "DriverLastName");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Remittances_RemittanceNumber",
                table: "Remittances");

            migrationBuilder.DropIndex(
                name: "IX_Debtors_DriverFirstName",
                table: "Debtors");

            migrationBuilder.DropIndex(
                name: "IX_Debtors_DriverLastName",
                table: "Debtors");

            migrationBuilder.AlterColumn<string>(
                name: "RemittanceNumber",
                table: "Remittances",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<int>(
                name: "ProductInsuranceNumber",
                table: "Remittances",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DriverLastName",
                table: "Debtors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "DriverFirstName",
                table: "Debtors",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
