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
            // IConfigurationRoot configuration = new ConfigurationBuilder()
            //     .SetBasePath(Directory.GetCurrentDirectory())
            //     .AddJsonFile("appsettings.json")
            //     .Build();
    
            var builder = new DbContextOptionsBuilder<NebulaContext>();
    
            // var connectionString = configuration.GetConnectionString("DefaultConnection");
    
            builder.UseSqlServer($"Server=localhost;Database=Nebula;User=sa;Password={Environment.GetEnvironmentVariable("DBPW")};");
    
            return new NebulaContext(builder.Options);
        }
    }
}