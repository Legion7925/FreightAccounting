using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreightAccounting.Core.Migrations
{
    /// <inheritdoc />
    public partial class AddedSubmitDateToDebtors : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "ProductInsuranceNumber",
                table: "Remittances",
                newName: "ProductInsurancePayment");

            migrationBuilder.AddColumn<DateTime>(
                name: "SubmitDate",
                table: "Debtors",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(2000, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SubmitDate",
                table: "Debtors");

            migrationBuilder.RenameColumn(
                name: "ProductInsurancePayment",
                table: "Remittances",
                newName: "ProductInsuranceNumber");
        }
    }
}
