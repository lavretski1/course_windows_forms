using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationProject.DAL
{
    public class RentContextFactory : IDesignTimeDbContextFactory<RentContext>
    {
        public RentContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<RentContext>();

            Console.WriteLine("Warning you should pass additional connection string parameter. \nExample: Update-Database -Args '\"your connection string here\"'");

            if (args.Length > 0)
            {
                optionsBuilder.UseSqlServer(args[0]);
            }
            else 
            {
                optionsBuilder.UseSqlServer("*");
            }

            return new RentContext(optionsBuilder.Options);
        }
    }
}
