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
            //create test data here:   
            context.Database.EnsureCreated(); 
           
            var correlation = new List<Correlation>();
            var documentsinfo = new List<DocumentInfo>();
            context.SaveChanges();
        }
    }
}
