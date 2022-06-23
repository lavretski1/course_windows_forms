using Microsoft.EntityFrameworkCore.Migrations;
using ApplicationProject.DAL.MigrationExtentions;

#nullable disable

namespace ApplicationProject.Migrations
{
    public partial class SignRegistration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateCertificateAndSignRegisterProcedureUp();
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateCertificateAndSignRegisterProcedureDown();
        }
    }
}
