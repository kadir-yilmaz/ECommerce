using Microsoft.Extensions.Configuration;
using System.IO;
using System;

namespace ECommerce.Persistence
{
    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {
                ConfigurationManager configurationManager = new();

                string basePath = AppContext.BaseDirectory;

                // Robust path resolution for appsettings.json:
                // 1. Check AppContext.BaseDirectory (where binaries are deployed, e.g. IIS publish folder or bin/Debug/...)
                // 2. Check Directory.GetCurrentDirectory()
                // 3. Check relative path to WebAPI project from CurrentDirectory (for EF Migrations run from Persistence folder)
                // 4. Check relative path to WebAPI project from BaseDirectory
                if (!File.Exists(Path.Combine(basePath, "appsettings.json")))
                {
                    if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json")))
                    {
                        basePath = Directory.GetCurrentDirectory();
                    }
                    else if (File.Exists(Path.Combine(Directory.GetCurrentDirectory(), "../../ECommerce.WebAPI/appsettings.json")))
                    {
                        basePath = Path.Combine(Directory.GetCurrentDirectory(), "../../ECommerce.WebAPI");
                    }
                    else if (File.Exists(Path.Combine(AppContext.BaseDirectory, "../../../ECommerce.WebAPI/appsettings.json")))
                    {
                        basePath = Path.Combine(AppContext.BaseDirectory, "../../../ECommerce.WebAPI");
                    }
                }

                configurationManager.SetBasePath(basePath);
                configurationManager.AddJsonFile("appsettings.json", optional: true);

                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                if (!string.IsNullOrEmpty(environmentName))
                {
                    configurationManager.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
                }

                configurationManager.AddEnvironmentVariables();

                return configurationManager.GetConnectionString("sqlConnection");
            }
        }
    }
}
