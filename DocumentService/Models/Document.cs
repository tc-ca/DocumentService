using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class Document
    {
        public Guid DocumentId { get; set; }
        public string FileName { get; set; }
        public DocumentTypes DocumentType { get; set; }
        public long DocumentSize { get; set; }
        public string Description { get; set; }
        public string FileType { get; set; }
        public string Language { get; set; }
        public string RequesterId { get; set; }
        public string UserCreatedById { get; set; }
        public string UserLastUpdatedById { get; set; }
        public string DeletedById { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateDeleted { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public string DocumentUrl { get; internal set; }
        public long FileSize { get; internal set; }
        public string SubmissionMethod { get; internal set; }
        public bool IsDeleted { get; set; }
    }
}
