using Microsoft.Extensions.Configuration;

namespace PoliceDataIngest.Services;

public static class ConfigService
{
    public static IConfigurationRoot Load()
    {
        return new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json", optional: false)
            .Build();
    }
}