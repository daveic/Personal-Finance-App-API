using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalFinanceAPI.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Balance",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BalDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ActBalance = table.Column<double>(type: "float", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Balance", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Bank",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iban = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankValue = table.Column<double>(type: "float", nullable: false),
                    BankNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Bank", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Credit",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    PrevDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CredValue = table.Column<double>(type: "float", nullable: false),
                    CredNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exp_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Credit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Debit",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DebInsDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RtNum = table.Column<double>(type: "float", nullable: false),
                    RtPaid = table.Column<double>(type: "float", nullable: false),
                    RemainToPay = table.Column<double>(type: "float", nullable: false),
                    Multiplier = table.Column<int>(type: "int", nullable: false),
                    RtFreq = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebValue = table.Column<double>(type: "float", nullable: false),
                    DebNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exp_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Debit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Deposit",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepValue = table.Column<double>(type: "float", nullable: false),
                    PercGrow = table.Column<double>(type: "float", nullable: false),
                    BankNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Expiration",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpDescription = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ExpValue = table.Column<double>(type: "float", nullable: false),
                    ExpDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ColorLabel = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Expiration", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "KnownMovement",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KMType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KMTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KMValue = table.Column<double>(type: "float", nullable: false),
                    KMNote = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Exp_ID = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_KnownMovement", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Ticket",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumTicket = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketValue = table.Column<double>(type: "float", nullable: false),
                    TicketNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ticket", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Transaction",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Usr_OID = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrsCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrsTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrsDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrsValue = table.Column<double>(type: "float", nullable: false),
                    TrsNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transaction", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Balance");

            migrationBuilder.DropTable(
                name: "Bank");

            migrationBuilder.DropTable(
                name: "Credit");

            migrationBuilder.DropTable(
                name: "Debit");

            migrationBuilder.DropTable(
                name: "Deposit");

            migrationBuilder.DropTable(
                name: "Expiration");

            migrationBuilder.DropTable(
                name: "KnownMovement");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Transaction");
        }
    }
}
