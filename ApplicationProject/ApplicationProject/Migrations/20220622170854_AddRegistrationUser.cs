using Microsoft.EntityFrameworkCore.Migrations;
using ApplicationProject.DAL.MigrationExtentions;

#nullable disable

namespace ApplicationProject.Migrations
{
    public partial class AddRegistrationUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddRegistrationUserUp();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddRegistrationUserDown();
        }
    }
}
