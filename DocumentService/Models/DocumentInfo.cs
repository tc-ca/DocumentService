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
        
        public string DESCRIPTION_TXT { get; set; }
        public int FILE_SIZE_NBR { get; set; }
        public string LANGUAGE_TXT { get; set; }
        public string FILE_NAME_NM { get; set; }
        public string DOCUMENT_URL { get; set; }
        public Guid CORRELATION_ID { get; set; }
        [Column(TypeName = "jsonb")]
        public string DOCUMENT_TYPES { get; set; }
        public string  SUBMISSION_METHOD_CD { get; set; }
        public string FILE_TYPE_CD { get; set; }
        public DateTime DATE_CREATED_DTE { get; set; }
        public string USER_CREATED_BY_ID { get; set; }
        public DateTime DATE_LAST_UPDATED_DTE { get; set; }
        public string USER_LAST_UPDATED_BY_ID { get; set; }
        public bool IS_DELETED_IND { get; set; }
        public DateTime DATE_DELETED_DTE { get; set; }
        public string DELETED_BY_ID { get; set; }
        [Key]
        public Guid DOCUMENT_ID { get; set; }
        public Correlation Correlation { get; set; }
    }
}
