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
            var correlation = new List<Correlation>();
            var documentsinfo = new List<DocumentInfo>();
     
            documents.Add(new Document {DocumentImage = new byte[0] });
          /*  correlation.Add(new Correlation { DateCreatedDte = DateTime.Today, DateLastUpdatedDte = DateTime.Today, TransactionCompleteInd = true, UserCreatedById = "Cory", UserLastUpdatedById = "billy" });
            documentsinfo.Add(new DocumentInfo { DateCreatedDte = correlation[0].DateCreatedDte, CorrelationId = correlation[0].CorrelationId, UserLastUpdatedById = correlation[0].UserLastUpdatedById, UserCreatedById = correlation[0].UserCreatedById, DateDeletedDte = DateTime.Now, DateLastUpdatedDte = correlation[0].DateLastUpdatedDte, DeletedById = "Billy", DescriptionTxt = "Doc", DocumentId = documents[0].DocumentId, DocumentTypes = "hh", FileNameNme = "hat", FileSizeNbr = 3, FileTypeCd = ".jpeg", IsDeletedInd = false, LanguageTxt = "EN-CA", SubmissionMethodCd = "Email" });
        */
           // documents.ForEach(d => context.Document.Add(d));
          /*  correlation.ForEach(d => context.Correlation.Add(d));
            documentsinfo.ForEach(d => context.DocumentInfo.Add(d));*/
          
            context.SaveChanges();
        }
    }
}
