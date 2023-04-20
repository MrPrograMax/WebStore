using Microsoft.EntityFrameworkCore;
using MyPracticWebStore.Models;

namespace MyPracticWebStore.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        { }

        public DbSet<Category> Category { get; set; }

    }
}
