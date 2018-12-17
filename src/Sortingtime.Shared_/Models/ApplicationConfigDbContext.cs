using Microsoft.EntityFrameworkCore;
using System.Configuration;
using System;

namespace Sortingtime.Models
{
    public class ApplicationConfigDbContext : ApplicationDbContext
    {
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString);
        }
    }
}
