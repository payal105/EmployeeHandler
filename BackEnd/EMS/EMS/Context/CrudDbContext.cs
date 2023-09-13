using EMS.Models;
using Microsoft.EntityFrameworkCore;

namespace EMS.Context
{
    public class CrudDbContext : DbContext
    {
        public CrudDbContext(DbContextOptions options) : base(options)
        {
        }

        public DbSet <Employee> Employees { get; set; }
    }
}
