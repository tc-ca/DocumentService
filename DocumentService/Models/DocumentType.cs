using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class DocumentType
    {
       
        public int DocumentTypecode { get; set; }
        [ForeignKey("Document")]
        public Guid DocumentId { get; set; }
        [Key]
        public Guid DocumentTypeId { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public int UserLastUpdatedById { get; set; }
        public int UserCreatedById { get; set; }
        public virtual Document Document { get; set; }
        public virtual DocumentInfo DocumentInfo { get; set; }

    }
}
