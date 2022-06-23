using Microsoft.EntityFrameworkCore.Migrations;
using ApplicationProject.DAL.MigrationExtentions;

#nullable disable

namespace ApplicationProject.Migrations
{
    public partial class AllowAdminToUseProcedures : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AllowAdminToUseAdminProceduresUp();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AllowAdminToUseAdminProceduresDown();
        }
    }
}
