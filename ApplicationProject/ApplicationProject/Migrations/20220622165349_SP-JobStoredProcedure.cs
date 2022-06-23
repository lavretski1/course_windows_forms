using Microsoft.EntityFrameworkCore.Migrations;
using ApplicationProject.DAL.MigrationExtentions;
#nullable disable

namespace ApplicationProject.Migrations
{
    public partial class SPJobStoredProcedure : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SP_JobStoredProcedureUp();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.SP_JobStoredProcedureDown();
        }
    }
}
