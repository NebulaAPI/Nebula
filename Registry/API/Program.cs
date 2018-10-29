using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Nebula.SDK.Objects;

namespace API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NebulaConfig.PluginDirectory = "/nebula/plugins";
            NebulaConfig.TemplateDirectory = "/nebula/templates";
            NebulaConfig.TempDirectory = "/nebula/tmp";
            
            CreateWebHostBuilder(args).Build().Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
