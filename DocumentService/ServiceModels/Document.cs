namespace DocumentService.ServiceModels
{
    using System;
    using System.Collections.Generic;
    using DocumentService.Models;

    public class Document
    {
        public Guid DocumentId { get; set; }

        public string FileName { get; set; }

        public string FileType { get; set; }

        public long FileSize { get; set; }

        public long DocumentSize => this.FileSize;

        public string DocumentUrl { get; set; }

        public string RequesterId { get; set; }

        public List<DocumentType> DocumentTypes { get; set; }

        public string Description { get; set; }

        public string Language { get; set; }

        public string SubmissionMethod { get; set; }

        public string UserCreatedById { get; set; }

        public DateTime? DateCreated { get; set; }

        public string UserLastUpdatedById { get; set; }

        public DateTime? DateLastUpdated { get; set; }

        public bool IsDeleted { get; set; }

        public DateTime? DateDeleted { get; set; }

        public string DeletedById { get; set; }
    }
}
