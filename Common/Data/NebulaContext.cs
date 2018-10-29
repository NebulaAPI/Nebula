using System;
using Microsoft.EntityFrameworkCore;
using Nebula.Common.Data.Models;
using Nebula.SDK.Objects.Server;

namespace Nebula.Common.Data
{
    public class NebulaContext : DbContext
    {
        public DbSet<Template> Templates { get; set; }
        public DbSet<Plugin> Plugins { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"Server=localhost;Database=Nebula;User=sa;Password={Environment.GetEnvironmentVariable("DBPW")};");
        }
    }
}