#if NETFULL
using Microsoft.EntityFrameworkCore;
using System.Configuration;

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
#endif