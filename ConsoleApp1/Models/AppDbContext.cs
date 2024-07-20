using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConsoleApp1.Models
{
    public class AppDbContext : DbContext
    {
        public DbSet<Product> Products { get; set; }
        public DbSet<Booking> Bookings { get; set; }
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) 
        {
            
        }
    }

    public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args)
        {
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer("Server=WIN-2J6811OHPJK\\SQLEXPRESS;Database=BookingOfGoodsDB;Trusted_Connection=True;Encrypt=False");

            return new AppDbContext(optionsBuilder.Options);
        }
    }
}
