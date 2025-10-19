using Microsoft.Extensions.Configuration;

namespace TestFilterConsoleApp.Tests;

public static class TestConfigurationBuilder
{
    public static IConfiguration Create(Dictionary<string, string> values)
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(values!)
            .Build();
    }
}