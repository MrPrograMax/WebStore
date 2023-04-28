using Microsoft.EntityFrameworkCore.Migrations;

namespace MyPracticWebStore_DataAccess.Migrations
{
    public partial class ChangeNickNameOnFullName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "NickName",
                table: "AspNetUsers",
                newName: "FullName");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "FullName",
                table: "AspNetUsers",
                newName: "NickName");
        }
    }
}
