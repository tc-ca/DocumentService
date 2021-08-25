namespace DocumentService.Tests.Common.Services
{
    using Microsoft.Extensions.Configuration;

    public class TestConfigurationBuilder : ITestConfigurationBuilder
    {
        public IConfiguration Build()
        {
            return new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
        }
    }
}
