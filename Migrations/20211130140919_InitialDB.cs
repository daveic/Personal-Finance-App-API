using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace PersonalFinanceAPI.Migrations
{
    public partial class InitialDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Bank",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Iban = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    BankValue = table.Column<int>(type: "int", nullable: false),
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
                    CredCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CredValue = table.Column<int>(type: "int", nullable: false),
                    CredNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    DebCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CredName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    DebDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DebInsDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RtNum = table.Column<int>(type: "int", nullable: false),
                    RtPaid = table.Column<int>(type: "int", nullable: false),
                    RemainToPay = table.Column<int>(type: "int", nullable: false),
                    DebValue = table.Column<int>(type: "int", nullable: false),
                    DebNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    BankName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    InitDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DepValue = table.Column<int>(type: "int", nullable: false),
                    PercGrow = table.Column<int>(type: "int", nullable: false),
                    BankNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Deposit", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "KnownMovement",
                columns: table => new
                {
                    ID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KMType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KMTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    KMValue = table.Column<int>(type: "int", nullable: false),
                    KMNote = table.Column<string>(type: "nvarchar(max)", nullable: true)
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
                    TicketName = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NumTicket = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TicketValue = table.Column<int>(type: "int", nullable: false),
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
                    TrsCode = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrsTitle = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TrsDateTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TrsValue = table.Column<int>(type: "int", nullable: false),
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
                name: "Bank");

            migrationBuilder.DropTable(
                name: "Credit");

            migrationBuilder.DropTable(
                name: "Debit");

            migrationBuilder.DropTable(
                name: "Deposit");

            migrationBuilder.DropTable(
                name: "KnownMovement");

            migrationBuilder.DropTable(
                name: "Ticket");

            migrationBuilder.DropTable(
                name: "Transaction");
        }
    }
}
