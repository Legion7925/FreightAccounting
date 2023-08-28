using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreightAccounting.Core.Migrations
{
    /// <inheritdoc />
    public partial class addedLicencePlateToDebtorTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DriverName",
                table: "Debtors",
                newName: "PlateNumber");

            migrationBuilder.RenameColumn(
                name: "DriverFamilyName",
                table: "Debtors",
                newName: "DriverLastName");

            migrationBuilder.AddColumn<string>(
                name: "DriverFirstName",
                table: "Debtors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DriverFirstName",
                table: "Debtors");

            migrationBuilder.RenameColumn(
                name: "PlateNumber",
                table: "Debtors",
                newName: "DriverName");

            migrationBuilder.RenameColumn(
                name: "DriverLastName",
                table: "Debtors",
                newName: "DriverFamilyName");
        }
    }
}
