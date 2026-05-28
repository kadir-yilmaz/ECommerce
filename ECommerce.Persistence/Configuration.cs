using Microsoft.Extensions.Configuration;

namespace ECommerce.Persistence
{
    static class Configuration
    {
        static public string ConnectionString
        {
            get
            {
                ConfigurationManager configurationManager = new();

                // WebAPI projesinin appsettings.json dosyasını bul
                string basePath = Path.Combine(Directory.GetCurrentDirectory(), "../../ECommerce.WebAPI");
                
                if (!Directory.Exists(basePath))
                    basePath = Directory.GetCurrentDirectory();

                configurationManager.SetBasePath(basePath);
                configurationManager.AddJsonFile("appsettings.json", optional: true);

                string environmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development";
                if (!string.IsNullOrEmpty(environmentName))
                {
                    configurationManager.AddJsonFile($"appsettings.{environmentName}.json", optional: true);
                }

                return configurationManager.GetConnectionString("sqlConnection");
            }
        }
    }
}
