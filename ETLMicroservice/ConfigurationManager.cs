using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Configuration.Yaml;

namespace ETLMicroservice
{
    // Handles configuration loading.
    public static class ConfigurationManager
    {
        public static IConfigurationRoot LoadConfiguration()
        {
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddYamlFile("config.yaml", optional: false, reloadOnChange: true);
            return builder.Build();
        }
    }
}