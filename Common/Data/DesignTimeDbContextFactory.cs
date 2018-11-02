using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace Nebula.Common.Data
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<NebulaContext>
    {
        public NebulaContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<NebulaContext>();
    
            builder.UseSqlServer($"Server=localhost;Database=Nebula;User=sa;Password={Environment.GetEnvironmentVariable("DBPW")};");
    
            return new NebulaContext(builder.Options);
        }
    }
}