using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    public class DocumentInfo
    {   
        [Key]
        public Guid DocumentInfoId{ get; set; }
        
        public Guid DocumentId { get; set; }
        
        public Guid DocumentTypeId { get; set; }
        [Required]
        public string Description { get; set; }
        [Required]
        public int File_Size { get; set; }
      
      
        [Required]
        public string Language { get; set; }
        [Required]
        public string FileName { get; set; }
        [Required]
        public string DocumentURL { get; set; }
        public DateTime DateCreated { get; set; }
        public int UserCreatedById { get; set; }
        public DateTime DateLastUpdated { get; set; }
        public int UserLastUpdatedById { get; set; }
       
        public virtual Document Document { get; set; }
       
        public virtual DocumentType DocumentType { get; set; }

    }
}
