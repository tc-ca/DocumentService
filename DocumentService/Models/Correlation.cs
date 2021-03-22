using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace DocumentService.Models
{
    //defines the correlation Table
    [Table("CORRELATION")]
    public class Correlation
    {
        /// <summary>
        /// Gets or sets the primary key
        /// </summary>
        [Column("CORRELATION_ID")]
        [Key]
        public Guid CorrelationId { get; set; }
       
        /// <summary>
        /// gets or sets the flag to see if the transaction is complete
        /// </summary>
        [Column("TRANSACTION_COMPLETE_IND")]
        public bool TransactionComplete { get; set; }

        /// <summary>
        /// gets or sets user who last updated the document
        /// </summary>
        [Column("USER_LAST_UPDATED_BY_ID")]
        public string UserLastUpdatedById { get; set; }

        /// <summary>
        /// gets or sets the date the file was modified
        /// </summary>
        [Column("DATE_LAST_UPDATED_DTE")]
        public DateTime DateLastUpdated { get; set; }

        /// <summary>
        /// gets or sets the user who created the document
        /// </summary>
        [Column("USER_CREATED_BY_ID")]
        public string UserCreatedById { get; set; }

        /// <summary>
        /// gets or sets date the document was created
        /// </summary>
        [Column("DATE_CREATED_DTE")]
        public DateTime DateCreated { get; set; }
    }
}
