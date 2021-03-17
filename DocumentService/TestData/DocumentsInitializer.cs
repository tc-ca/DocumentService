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
        private readonly string[] names = { "Cory", "Alexandre", "Tibo", "Chris", "Paul", "Mansuer" };
        public DocumentContext context { get; set; }
        Random ran = new Random();
      /*  public DocumentsInitializer(DocumentContext context)
        {
            
        }*/

        
        public void Seed()
        {
          
           // context.Database.EnsureDeleted();
           context.Database.EnsureCreated();

            var documents = new List<Document>() {
                new Document { CreateBy = "Cory", CreatedDate = DateTime.Now, ModifiedBy = "Alexandre", ModifiedDate = DateTime.Today, FileBlob = new byte[0] },
                new Document { CreateBy = "Alexandre", CreatedDate = DateTime.Now, ModifiedBy = "Cory", ModifiedDate = DateTime.Today, FileBlob = new byte[0] }
            };

            documents.ForEach(d => context.Documents.Add(d));
            context.SaveChanges();
        }
    }
}
