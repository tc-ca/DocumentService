using DocumentService.Azure;
using DocumentService.Repositories;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DocumentService.Unit.Tests.Services
{
    public class ServiceInitializer
    {
        private readonly IDocumentRepository documentRepository;

        private readonly IAzureBlobService azureBlobService;

        private readonly IConfiguration configuration;

        public ServiceInitializer(IConfiguration configuration)
        {
            this.configuration = new ConfigurationBuilder().AddJsonFile("appsettings.test.json").Build();
        }


    }
}
