using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LoadDataService.Migrations
{
    public partial class InitialCatalog : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Session",
                columns: table => new
                {
                    SessionId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ClientId = table.Column<string>(nullable: true),
                    StringQueryPMI = table.Column<string>(nullable: true),
                    RowSize = table.Column<int>(nullable: false),
                    PreviousPages = table.Column<string>(nullable: true),
                    CurrentPage = table.Column<int>(nullable: false),
                    LastModification = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Session", x => x.SessionId);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Session");
        }
    }
}
