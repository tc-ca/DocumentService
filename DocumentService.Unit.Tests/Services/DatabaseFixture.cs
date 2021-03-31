using DocumentService.Contexts;
using DocumentService.TestData;
using Microsoft.Extensions.Configuration;
using System;
using Xunit;

namespace DocumentService.Unit.Tests.Services
{
    [CollectionDefinition("Database collection")]
    public class DatabaseFixture : IDisposable, ICollectionFixture<DatabaseFixture>
    {
        public DocumentContext Context { get; set; }

        private IConfiguration configuration;

        public DatabaseFixture()
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
            this.Context = new DocumentContext(this.configuration);
            IDocumentsInitializer documentInitializer = new DocumentsInitializer();
            documentInitializer.context = this.Context;
            documentInitializer.Seed();
        }

        public void Dispose()
        {
            this.Context.Database.EnsureDeleted();
        }
    }
}
