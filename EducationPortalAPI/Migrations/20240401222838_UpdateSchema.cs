using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EducationPortalAPI.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "UserID",
                table: "Participations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int"); 

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Educations",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int"); 
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "UserID",
                table: "Participations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"); 

            migrationBuilder.AlterColumn<int>(
                name: "UserId",
                table: "Educations",
                type: "int",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)"); 
        }
    }
}
