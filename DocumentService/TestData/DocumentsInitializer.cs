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
        
        var documents = new List<Document>();
        var documentsinfo = new List<DocumentInfo>();
        var documentstype = new List<DocumentType>();
        documents.Add(new Document {DocumentImage = new byte[0] });
      /*  documentsinfo.Add(new DocumentInfo { Description = "Hat", File_Size = 0, File_Name = "Pat", Language = "EN", Document_URL = "www.com", Date_Created = DateTime.Today,
                                             User_Created_By_Id = 0, Date_Last_Updated = DateTime.Today, User_Last_Updated_By_Id = 0 });

        documentstype.Add(new DocumentType { })*/

            documents.ForEach(d => context.Document.Add(d));
            context.SaveChanges();
        }
    }
}
