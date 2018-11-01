using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using Nebula.SDK.Objects.Server;
using Nebula.SDK.Objects.Shared;

namespace Nebula.Common.Data
{
    public class NebulaContext : DbContext
    {
        public DbSet<Template> Templates { get; set; }
        public DbSet<Plugin> Plugins { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserApiKey> ApiKeys { get; set; }

        public NebulaContext(DbContextOptions<NebulaContext> options) : base(options) { }
        
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.UseSqlServer($"Server=localhost;Database=Nebula;User=sa;Password={Environment.GetEnvironmentVariable("DBPW")};");
        }

        public List<Plugin> GetPlugins()
        {
            return Plugins
                .Include(p => p.UploadedBy)
                .Include(p => p.Versions)
                    .ThenInclude(version => version.Dependencies)
                .ToList();
        }

        public List<Plugin> QueryPlugins(Expression<Func<Plugin, bool>> predicate)
        {
            return Plugins
                .Include(p => p.UploadedBy)
                .Include(p => p.Versions)
                    .ThenInclude(version => version.Dependencies)
                .Where(predicate)
                .ToList();
        }
    }
}