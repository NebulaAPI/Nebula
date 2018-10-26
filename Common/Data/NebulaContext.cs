using Microsoft.EntityFrameworkCore;
using Nebula.Common.Data.Models;

namespace Nebula.Common.Data
{
    public class NebulaContext : DbContext
    {
        public DbSet<Template> Templates { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer("Server=localhost;Database=Nebula;User=sa;Password=;");
        }
    }
}