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
        public Guid DocumentId { get; set; }
        public string DescriptionTxt { get; set; }
        public string FileNameNme { get; set; }
        public string SubmissionMethodCd { get; set; }
        public string FileTypeCd { get; set; }
        public int FileSizeNbr { get; set; }
        public string LanguageTxt { get; set; }
        [Column(TypeName ="jsonb")]
        public string DocumentTypes { get; set; }
        public Guid CorrelationId { get; set; }
        public DateTime DateCreatedDte { get; set; }
        public string UserCreatedById { get; set; }
        public DateTime DateLastUpdatedDte { get; set; }
        public string UserLastUpdatedById { get; set; }
        public bool IsDeletedInd { get; set; }
        public DateTime DateDeletedDte { get; set; }
        public string DeletedById { get; set; }
        public Document Document { get; set; }
        public Correlation Correlation { get; set; }
    }
}
