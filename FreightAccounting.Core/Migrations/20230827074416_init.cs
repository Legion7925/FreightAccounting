using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FreightAccounting.Core.Migrations
{
    /// <inheritdoc />
    public partial class init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Debtors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DriverName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    DriverFamilyName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Destination = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebtAmount = table.Column<int>(type: "int", nullable: false),
                    PaymentDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debtors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Remittances",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    RemittanceNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TransforPayment = table.Column<int>(type: "int", nullable: false),
                    OrganizationPayment = table.Column<int>(type: "int", nullable: false),
                    InsurancePayment = table.Column<int>(type: "int", nullable: false),
                    TaxPayment = table.Column<int>(type: "int", nullable: false),
                    SubmittedUsername = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NetProfit = table.Column<int>(type: "int", nullable: false),
                    ReceviedCommission = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Remittances", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Debtors");

            migrationBuilder.DropTable(
                name: "Remittances");
        }
    }
}
