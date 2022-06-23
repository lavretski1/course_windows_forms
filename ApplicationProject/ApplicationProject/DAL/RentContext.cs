using ApplicationProject.DAL.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationProject.DAL
{
    public class RentContext : DbContext
    {
        public DbSet<Models.Room> Rooms { get; set; }
        public DbSet<Models.RoomType> RoomTypes { get; set; }
        public DbSet<Tenant> Tenants { get; set; }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<RentAccounting> Rents { get; set; }
        public DbSet<Admin> Admins { get; set; }

        public RentContext() : base() { }

        public RentContext(DbContextOptions options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Tenant>()
                .HasIndex(t => t.Username)
                .IsUnique();
        }

        public void ToggleAccess(string username) 
        {
            var usernameParameter = new SqlParameter("@Username", System.Data.SqlDbType.NVarChar);
            usernameParameter.Value = username;

            this.Database.ExecuteSqlRaw("exec ToggleAccess @Username", usernameParameter);
        }

        public void CreateAdmin(string username, string password)
        {
            var usernameParameter = new SqlParameter("@Username", System.Data.SqlDbType.NVarChar);
            usernameParameter.Value = username;
            var passwordParameter = new SqlParameter("@Password", System.Data.SqlDbType.NVarChar);
            passwordParameter.Value = password;

            this.Database.ExecuteSqlRaw("exec CreateAdmin @Username, @Password", usernameParameter, passwordParameter);
        }

        public void DeleteUser(string username)
        {
            var usernameParameter = new SqlParameter("@Username", System.Data.SqlDbType.NVarChar);
            usernameParameter.Value = username;

            this.Database.ExecuteSqlRaw("exec DeleteUser @Username", usernameParameter);
        }

        public static string Register(string username, string password, string name, string legalAddress, string bankName, string head, string characteristic)
        {
            var usernameParameter = new SqlParameter("@Username", System.Data.SqlDbType.NVarChar);
            usernameParameter.Value = username;
            var passwordParameter = new SqlParameter("@Password", System.Data.SqlDbType.NVarChar);
            passwordParameter.Value = password;
            var nameParameter = new SqlParameter("@Name", System.Data.SqlDbType.NVarChar);
            nameParameter.Value = name;
            var legalAddressParameter = new SqlParameter("@LegalAddress", System.Data.SqlDbType.NVarChar);
            legalAddressParameter.Value = legalAddress;
            var bankNameParameter = new SqlParameter("@BankName", System.Data.SqlDbType.NVarChar);
            bankNameParameter.Value = bankName;
            var headParameter = new SqlParameter("@Head", System.Data.SqlDbType.NVarChar);
            headParameter.Value = head;
            var characteristicParameter = new SqlParameter("@Characteristic", System.Data.SqlDbType.NVarChar);
            characteristicParameter.Value = characteristic;
            var resultParameter = new SqlParameter("@Result", System.Data.SqlDbType.NVarChar);
            resultParameter.Direction = System.Data.ParameterDirection.Output;
            resultParameter.Size = 2048;

            DbContextOptionsBuilder builder = new DbContextOptionsBuilder();

            builder.UseSqlServer(ConfigurationManager.ConnectionStrings["register"].ConnectionString);

            using (RentContext context = new RentContext(builder.Options))
            {
                context.Database.ExecuteSqlRaw("exec Register @Username, @Password, @Name, @LegalAddress, @BankName, @Head, @Characteristic, @Result output",
                    usernameParameter,
                    passwordParameter,
                    nameParameter,
                    legalAddressParameter,
                    bankNameParameter,
                    headParameter,
                    characteristicParameter,
                    resultParameter);
            }
            return resultParameter.Value as string;
        }
    }
}
