using Microsoft.EntityFrameworkCore;
using Sortingtime.Models;

namespace Sortingtime.PdfMailWebJob.Model
{
    public class ApplicationConfigDbContext : ApplicationDbContext
    {
        private readonly string connectionString;

        public ApplicationConfigDbContext(string connectionString)
        {
            this.connectionString = connectionString;
        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}
