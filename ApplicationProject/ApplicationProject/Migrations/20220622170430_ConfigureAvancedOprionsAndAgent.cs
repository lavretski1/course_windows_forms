using Microsoft.EntityFrameworkCore.Migrations;
using ApplicationProject.DAL.MigrationExtentions;

#nullable disable

namespace ApplicationProject.Migrations
{
    public partial class ConfigureAvancedOprionsAndAgent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.ConfigureAdvancedOpdionsAndAgentUp();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.ConfigureAdvancedOpdionsAndAgentDown();
        }
    }
}
