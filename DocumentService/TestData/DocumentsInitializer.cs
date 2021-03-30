using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DocumentService.Models;
using DocumentService.Contexts;

namespace DocumentService.TestData
{
    public class DocumentsInitializer : IDocumentsInitializer
    {
        /// <summary>
        /// Gets or sets the context for the documentService database
        /// </summary>
        public DocumentContext context { get; set; }


        public void Seed()
        {
            Guid id = Guid.NewGuid();
            //create test data here:   
            context.Database.EnsureCreated();

            var correlation = new Correlation
            {
                CorrelationId = id,
                DateCreated = DateTime.Now,
                DateLastUpdated = DateTime.Now,
                TransactionComplete = true,
                UserCreatedById = "hat",
                UserLastUpdatedById = "Billy"
            };

            var documentsinfo = new DocumentInfo
            {
                CorrelationId = id,
                DateCreated = DateTime.Now,
                Description = "HAtty hat",
                FileName = "yomp",
                DocumentTypes = new DocumentTypes { DocType = "Typedoc", DocumentTypesId = 1 },
                IsDeleted = false

            };


            context.Add(correlation);
            context.Add(documentsinfo);

            context.SaveChanges();
        }
    }
}
