using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace DocumentService.Models
{
    /*
     * Correlation entity
     * Columns:
     * CORRELATION_ID : GUID
     * TRANSACTION_COMPLETE_IND : BOOL
     * USER_LAST_UPDATED_BY_ID : STRING
     * DATE_LAST_UPDATED_DTE: DATETIME
     * USER_CREATED_BY_ID : STRING
     * DATE_CREATED_DTE : DATETIME
     */
    public class Correlation
    {
        [Key]
        public Guid CORRELATION_ID { get; set; } //primary key
        public bool TRANSACTION_COMPLETE_IND { get; set; } //flag to set if the transaction is complete
        public string USER_LAST_UPDATED_BY_ID { get; set; } //user who last updated the document
        public DateTime DATE_LAST_UPDATED_DTE { get; set; } //date the file was modified
        public string USER_CREATED_BY_ID { get; set; } //user who created the document
        public DateTime DATE_CREATED_DTE { get; set; } //date the document was created
    }
}
