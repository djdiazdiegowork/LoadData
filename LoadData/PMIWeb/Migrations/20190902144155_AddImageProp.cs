using Microsoft.EntityFrameworkCore.Migrations;

namespace PMIWeb.Migrations
{
    public partial class AddImageProp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ImageCustomName",
                table: "AspNetUsers",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImageRealName",
                table: "AspNetUsers",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageCustomName",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "ImageRealName",
                table: "AspNetUsers");
        }
    }
}
