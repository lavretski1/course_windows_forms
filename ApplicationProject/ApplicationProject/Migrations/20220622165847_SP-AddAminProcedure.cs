using Microsoft.EntityFrameworkCore.Migrations;
using ApplicationProject.DAL.MigrationExtentions;

#nullable disable

namespace ApplicationProject.Migrations
{
    public partial class SPAddAminProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SP_AddAdminUp();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SP_AddAdminDown();
        }
    }
}
