using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalFinanceAPI.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "TrsValue",
                table: "Transaction",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Usr_OID",
                table: "Transaction",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "TicketValue",
                table: "Ticket",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Usr_OID",
                table: "Ticket",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "KMValue",
                table: "KnownMovement",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Usr_OID",
                table: "KnownMovement",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "PercGrow",
                table: "Deposit",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "DepValue",
                table: "Deposit",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Usr_OID",
                table: "Deposit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "RtPaid",
                table: "Debit",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "RtNum",
                table: "Debit",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "RemainToPay",
                table: "Debit",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<double>(
                name: "DebValue",
                table: "Debit",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "Multiplier",
                table: "Debit",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "RtFreq",
                table: "Debit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Usr_OID",
                table: "Debit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "CredValue",
                table: "Credit",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<DateTime>(
                name: "PrevDateTime",
                table: "Credit",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "Usr_OID",
                table: "Credit",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "BankValue",
                table: "Bank",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Usr_OID",
                table: "Bank",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "ActBalance",
                table: "Balance",
                type: "float",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<string>(
                name: "Usr_OID",
                table: "Balance",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Expiration",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpValue = table.Column<double>(type: "float", nullable: false),
                    input_value = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpDescription = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expiration", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Expiration");

            migrationBuilder.DropColumn(
                name: "Usr_OID",
                table: "Transaction");

            migrationBuilder.DropColumn(
                name: "Usr_OID",
                table: "Ticket");

            migrationBuilder.DropColumn(
                name: "Usr_OID",
                table: "KnownMovement");

            migrationBuilder.DropColumn(
                name: "Usr_OID",
                table: "Deposit");

            migrationBuilder.DropColumn(
                name: "Multiplier",
                table: "Debit");

            migrationBuilder.DropColumn(
                name: "RtFreq",
                table: "Debit");

            migrationBuilder.DropColumn(
                name: "Usr_OID",
                table: "Debit");

            migrationBuilder.DropColumn(
                name: "PrevDateTime",
                table: "Credit");

            migrationBuilder.DropColumn(
                name: "Usr_OID",
                table: "Credit");

            migrationBuilder.DropColumn(
                name: "Usr_OID",
                table: "Bank");

            migrationBuilder.DropColumn(
                name: "Usr_OID",
                table: "Balance");

            migrationBuilder.AlterColumn<int>(
                name: "TrsValue",
                table: "Transaction",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "TicketValue",
                table: "Ticket",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "KMValue",
                table: "KnownMovement",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "PercGrow",
                table: "Deposit",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "DepValue",
                table: "Deposit",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "RtPaid",
                table: "Debit",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "RtNum",
                table: "Debit",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "RemainToPay",
                table: "Debit",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "DebValue",
                table: "Debit",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "CredValue",
                table: "Credit",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "BankValue",
                table: "Bank",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<int>(
                name: "ActBalance",
                table: "Balance",
                type: "int",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }
    }
}
