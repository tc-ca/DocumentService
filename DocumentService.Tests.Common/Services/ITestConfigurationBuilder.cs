namespace DocumentService.Tests.Common.Services
{
    using Microsoft.Extensions.Configuration;
    public interface ITestConfigurationBuilder
    {
        IConfiguration Build();
    }
}
