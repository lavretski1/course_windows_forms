using Microsoft.EntityFrameworkCore.Migrations;
using ApplicationProject.DAL.MigrationExtentions;

#nullable disable

namespace ApplicationProject.Migrations
{
    public partial class AddJob : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddJobUp();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddJobDown();
        }
    }
}
