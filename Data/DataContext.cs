using Microsoft.EntityFrameworkCore;
using SampleMiddlewareProject.Models;

namespace SampleMiddlewareProject.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> dbContextOptions) : base(dbContextOptions)
        {

        }

        public DbSet<Person> Persons { get; set; }
    }
}
