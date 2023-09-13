using EMS.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.Context
{
    public class AddDbContext : DbContext
    {
        public AddDbContext(DbContextOptions<AddDbContext> options):base(options)
        {

        }

        public DbSet<User> Users { get; set; }
        public DbSet<Employee> Employees { get; set; }
        public DbSet<Feedback> Feedbacks { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>().ToTable("users");
            modelBuilder.Entity<Employee>().ToTable("employees");
            modelBuilder.Entity<Feedback>().ToTable("feedback");
        }
    }
}
